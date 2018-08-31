using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HBitcoin.KeyManagement;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace Wallet.Core.CoinProviders
{
    public class BitcoinProvider : CoinProvider
    {
        public Network CurrentNetwork { get; set; }

        public BitcoinProvider(Network network)
        {
            CurrentNetwork = network;
        }

        public override decimal GetUSDBalance()
        {
            var url = "https://api.coinmarketcap.com/v1/ticker/bitcoin/?convert=usd";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            var response = request.GetResponse() as HttpWebResponse;
            string result;
            using (var responseStream = response.GetResponseStream())
            {
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }

            dynamic dynObj = JsonConvert.DeserializeObject(result);

            return decimal.Parse(dynObj[0].price_usd.ToString());

        }

        public override void SetNetwork(NetworkType network)
        {
            switch (network)
            {
                case NetworkType.MainNet:
                    CurrentNetwork = Network.Main;
                    break;
                case NetworkType.TestNet:
                    CurrentNetwork = Network.TestNet;
                    break;
            }
        }


        public Dictionary<BitcoinAddress, List<BalanceOperation>> QueryOperationsPerSafeAddresses(Safe safe, int minUnusedKeys = 7, HdPathType? hdPathType = null)
        {
            if (hdPathType == null)
            {
                var operationsPerReceiveAddresses = QueryOperationsPerSafeAddresses(safe, 7, HdPathType.Receive);
                var operationsPerChangeAddresses = QueryOperationsPerSafeAddresses(safe, 7, HdPathType.Change);

                var operationsPerAllAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
                foreach (var elem in operationsPerReceiveAddresses)
                    operationsPerAllAddresses.Add(elem.Key, elem.Value);
                foreach (var elem in operationsPerChangeAddresses)
                    operationsPerAllAddresses.Add(elem.Key, elem.Value);
                return operationsPerAllAddresses;
            }

            var addresses = safe.GetFirstNAddresses(minUnusedKeys, hdPathType.GetValueOrDefault());
            //var addresses = FakeData.FakeSafe.GetFirstNAddresses(minUnusedKeys);

            var operationsPerAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
            var unusedKeyCount = 0;
            foreach (var elem in QueryOperationsPerAddresses(addresses))
            {
                operationsPerAddresses.Add(elem.Key, elem.Value);
                if (elem.Value.Count == 0) unusedKeyCount++;
            }

            var startIndex = minUnusedKeys;
            while (unusedKeyCount < minUnusedKeys)
            {
                addresses = new List<BitcoinAddress>();
                for (var i = startIndex; i < startIndex + minUnusedKeys; i++)
                {
                    addresses.Add(safe.GetAddress(i, hdPathType.GetValueOrDefault()));
                    //addresses.Add(FakeData.FakeSafe.GetAddress(i));
                }
                foreach (var elem in QueryOperationsPerAddresses(addresses))
                {
                    operationsPerAddresses.Add(elem.Key, elem.Value);
                    if (elem.Value.Count == 0) unusedKeyCount++;
                }
                startIndex += minUnusedKeys;
            }

            return operationsPerAddresses;
        }

        public Dictionary<BitcoinAddress, List<BalanceOperation>> QueryOperationsPerAddresses(IEnumerable<BitcoinAddress> addresses)
        {
            var operationsPerAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
            var client = new QBitNinjaClient(CurrentNetwork);
            foreach (var addr in addresses)
            {
                var operations = client.GetBalance(addr, unspentOnly: false).Result.Operations;
                operationsPerAddresses.Add(addr, operations);
            }
            return operationsPerAddresses;
        }

        public Dictionary<Coin, bool> GetUnspentCoins(IEnumerable<ISecret> secrets)
        {
            var unspentCoins = new Dictionary<Coin, bool>();
            foreach (var secret in secrets)
            {
                var destination = secret.PrivateKey.ScriptPubKey.GetDestinationAddress(CurrentNetwork);

                var client = new QBitNinjaClient(CurrentNetwork);
                var balanceModel = client.GetBalance(destination, unspentOnly: true).Result;
                foreach (var operation in balanceModel.Operations)
                {
                    foreach (var elem in operation.ReceivedCoins.Select(coin => coin as Coin))
                    {
                        unspentCoins.Add(elem, operation.Confirmations > 0);
                    }
                }
            }

            return unspentCoins;
        }

        private Money ParseBtcString(string value)
        {
            decimal amount;
            if (!decimal.TryParse(
                value.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out amount))
            {
            }


            return new Money(amount, MoneyUnit.BTC);
        }

        public bool SelectCoins(ref HashSet<Coin> coinsToSpend, Money totalOutAmount, List<Coin> unspentCoins)
        {
            var haveEnough = false;
            foreach (var coin in unspentCoins.OrderByDescending(x => x.Amount))
            {
                coinsToSpend.Add(coin);
                // if doesn't reach amount, continue adding next coin
                if (coinsToSpend.Sum(x => x.Amount) < totalOutAmount) continue;
                else
                {
                    haveEnough = true;
                    break;
                }
            }

            return haveEnough;
        }

        public override Transaction SendTransaction(SendTransaction txx)
        {
            var walletFilePath = System.Environment.CurrentDirectory + $"\\bitcoin{WalletName}.json";
            var safe = Safe.Load(Password, walletFilePath);
            BitcoinAddress addressToSend;
            try
            {
                addressToSend = BitcoinAddress.Create(txx.Receiver, CurrentNetwork);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var operationsPerAddresses = QueryOperationsPerSafeAddresses(safe, 7);

            var operationsPerNotEmptyPrivateKeys = new Dictionary<BitcoinExtKey, List<BalanceOperation>>();
            foreach (var elem in operationsPerAddresses)
            {
                var balance = Money.Zero;
                foreach (var op in elem.Value) balance += op.Amount;
                if (balance <= Money.Zero) continue;
                var secret = safe.FindPrivateKey(elem.Key);
                operationsPerNotEmptyPrivateKeys.Add(secret, elem.Value);
            }

            Script changeScriptPubKey = null;
            var operationsPerChangeAddresses = QueryOperationsPerSafeAddresses(safe, minUnusedKeys: 1, hdPathType: HdPathType.Change);
            foreach (var elem in operationsPerChangeAddresses)
            {
                if (elem.Value.Count == 0)
                    changeScriptPubKey = safe.FindPrivateKey(elem.Key).ScriptPubKey;
            }
            if (changeScriptPubKey == null)
                throw new ArgumentNullException();

            var unspentCoins = GetUnspentCoins(operationsPerNotEmptyPrivateKeys.Keys);

            Money fee;
            try
            {
                const int txSizeInBytes = 250;
                using (var client = new HttpClient())
                {

                    const string request = @"https://bitcoinfees.21.co/api/v1/fees/recommended";
                    var result = client.GetAsync(request, HttpCompletionOption.ResponseContentRead).Result;
                    var json = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    var fastestSatoshiPerByteFee = json.Value<decimal>("fastestFee");
                    fee = new Money(fastestSatoshiPerByteFee * txSizeInBytes, MoneyUnit.Satoshi);
                }
            }
            catch
            {
                throw new Exception("Can't get tx fee");
            }

            var availableAmount = Money.Zero;
            var unconfirmedAvailableAmount = Money.Zero;
            foreach (var elem in unspentCoins)
            {
                if (true)  //todo
                {
                    availableAmount += elem.Key.Amount;
                    if (!elem.Value)
                        unconfirmedAvailableAmount += elem.Key.Amount;
                }
                else
                {
                    if (elem.Value)
                    {
                        availableAmount += elem.Key.Amount;
                    }
                }
            }

            var amountToSend = ParseBtcString(txx.Amount.ToString(CultureInfo.InvariantCulture));
            var totalOutAmount = amountToSend + fee;

            var coinsToSpend = new HashSet<Coin>();
            var unspentConfirmedCoins = new List<Coin>();
            var unspentUnconfirmedCoins = new List<Coin>();
            foreach (var elem in unspentCoins)
                if (elem.Value) unspentConfirmedCoins.Add(elem.Key);
                else unspentUnconfirmedCoins.Add(elem.Key);

            var haveEnough = SelectCoins(ref coinsToSpend, totalOutAmount, unspentConfirmedCoins);
            if (!haveEnough)
                haveEnough = SelectCoins(ref coinsToSpend, totalOutAmount, unspentUnconfirmedCoins);
            if (!haveEnough)
                throw new Exception("Not enough funds.");

            var signingKeys = new HashSet<ISecret>();
            foreach (var coin in coinsToSpend)
            {
                foreach (var elem in operationsPerNotEmptyPrivateKeys)
                {
                    if (elem.Key.ScriptPubKey == coin.ScriptPubKey)
                        signingKeys.Add(elem.Key);
                }
            }

            var builder = new TransactionBuilder();
            var tx = builder
                .AddCoins(coinsToSpend)
                .AddKeys(signingKeys.ToArray())
                .Send(addressToSend, amountToSend)
                .SetChange(changeScriptPubKey)
                .SendFees(fee)
                .BuildTransaction(true);



            var qBitClient = new QBitNinjaClient(CurrentNetwork);

            BroadcastResponse broadcastResponse;
            var success = false;
            var tried = 0;
            const int maxTry = 7;
            do
            {
                tried++;
                broadcastResponse = qBitClient.Broadcast(tx).Result;
                var getTxResp = qBitClient.GetTransaction(tx.GetHash()).Result;
                if (getTxResp == null)
                {
                    Thread.Sleep(3000);
                    continue;
                }
                else
                {
                    success = true;
                    var transaction = new Transaction();
                    transaction.Hash = getTxResp.TransactionId.ToString();
                    transaction.Text =
                        $"Transaction ID: {getTxResp.TransactionId}, sent coins {txx.Amount.ToString()}";
                    transaction.Value = txx.Amount;

                    return transaction;
                }
            } while (tried <= maxTry);

            return null;
        }

        public Dictionary<uint256, List<BalanceOperation>> GetOperationsPerTransactions(Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerAddresses)
		{
			var opSet = new HashSet<BalanceOperation>();
			foreach (var elem in operationsPerAddresses)
				foreach (var op in elem.Value)
					opSet.Add(op);
			if (!opSet.Any()) 
                return new Dictionary<uint256, List<BalanceOperation>>();

			var operationsPerTransactions = new Dictionary<uint256, List<BalanceOperation>>();
			foreach (var op in opSet)
			{
				var txId = op.TransactionId;
                if (operationsPerTransactions.TryGetValue(txId, out List<BalanceOperation> ol))
                {
                    ol.Add(op);
                    operationsPerTransactions[txId] = ol;
                }
                else operationsPerTransactions.Add(txId, new List<BalanceOperation> { op });
            }

			return operationsPerTransactions;
		}

        public override List<Transaction> GetWalletHistory()
        {
            var path = Environment.CurrentDirectory + $"\\bitcoin{WalletName}.json";

            var safe = Safe.Load(Password, path);
            var txs = new List<Transaction>();
      
            var operationsPerAddresses = QueryOperationsPerSafeAddresses(safe);
            var operationsPerTransactions = GetOperationsPerTransactions(operationsPerAddresses);

            var txHistoryRecords = new List<Tuple<DateTimeOffset, Money, int, uint256>>();
            foreach (var elem in operationsPerTransactions)
            {
                var amount = Money.Zero;
                foreach (var op in elem.Value)
                    amount += op.Amount;
                var firstOp = elem.Value.First();

                txHistoryRecords
                    .Add(new Tuple<DateTimeOffset, Money, int, uint256>(
                        firstOp.FirstSeen,
                        amount,
                        firstOp.Confirmations,
                        elem.Key));
            }

            var orderedTxHistoryRecords = txHistoryRecords
                .OrderByDescending(x => x.Item1); // Confirmations
            foreach (var record in orderedTxHistoryRecords)
            {
                var tx = new Transaction()
                {
                    Value = decimal.Parse(record.Item2.Satoshi.ToString()),
                    Hash = record.Item4.ToString()
                };

                if (record.Item2 > 0)
                {
                    //todo add colors
                    tx.Text = $"Transaction ID: {record.Item4.ToString()}, received coins {record.Item2.ToString()}, confirms: {record.Item3}";
                }
                else if (record.Item2 < 0)
                {
                    tx.Text = $"Transaction ID: {record.Item4.ToString()}, sent coins {record.Item2.ToString()}, confirms: {record.Item3}";
                }
                txs.Add(tx);
            }

            return txs;
        }

        public override void RestoreWallet(string walletName)
        {

            //            Safe.Recover(mnemonic, pw, walletFilePath + "RecoverdWallet" + rand.Next(), Network.TestNet, DateTimeOffset.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            //            Safe.Recover(new Mnemonic(Mnemonic), password, $@".\{walletName}", global::NBitcoin.Network.TestNet);

        }

        public override decimal GetBalance()
        {
            try
            {
                var path = System.Environment.CurrentDirectory + $"\\bitcoin{WalletName}.json";
                var wallet = Safe.Load(Password, path);
                var client = new QBitNinjaClient(CurrentNetwork);
                decimal totalBalance = 0;
                WalletAddress = wallet.GetAddress(0).ToString();
                var nbitcoinAddress = BitcoinAddress.Create(WalletAddress, CurrentNetwork);
                var balance = client.GetBalance(nbitcoinAddress, true).Result;
                foreach (var entry in balance.Operations)
                {
                    foreach (var coin in entry.ReceivedCoins)
                    {
                        var amount = (Money)coin.Amount;
                        var currentAmount = amount.ToDecimal(MoneyUnit.BTC);
                        totalBalance += currentAmount;
                    }
                }

                return totalBalance;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
