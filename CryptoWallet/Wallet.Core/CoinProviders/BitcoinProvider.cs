using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HBitcoin.KeyManagement;
using NBitcoin;
using Newtonsoft.Json;
using QBitNinja.Client;

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

        public override void SendTransaction()
        {
            throw new NotImplementedException();
        }

        public override List<Transaction> GetWalletHistory()
        {
            var path = Environment.CurrentDirectory + $"\\bitcoin{WalletName}.json";

            Safe.Load(Password, path);
            var client = new QBitNinjaClient(Network.TestNet);
            var receivedCoins = client.GetBalance(BitcoinAddress.Create(WalletAddress), true).Result;
            var txs = new List<Transaction>();
            foreach (var entry in receivedCoins.Operations)
            {
                foreach (var coin in entry.ReceivedCoins)
                {
                   txs.Add(
                       new Transaction()
                       {
                           Hash = coin.Outpoint.Hash.ToString(),
                           Value = decimal.Parse(coin.Amount.ToString()),
                           Text = $"Transaction ID: {coin.Outpoint.Hash}, received coins {coin.Amount.ToString()}"
                       });
                }
            }

            return txs;
        }

        public override void RestoreWallet(string walletName)
        {

            //            Safe.Recover(mnemonic, pw, walletFilePath + "RecoverdWallet" + rand.Next(), Network.TestNet, DateTimeOffset.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture));
//            Safe.Recover(new Mnemonic(Mnemonic), password, $@".\{walletName}", global::NBitcoin.Network.TestNet);

        }

        public  override decimal GetBalance()
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
