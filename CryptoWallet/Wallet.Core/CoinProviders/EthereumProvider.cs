using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Rijndael256;
using BigInteger = NBitcoin.BouncyCastle.Math.BigInteger;

namespace Wallet.Core.CoinProviders
{
    public class EthereumProvider : CoinProvider
    {
        public string CurrentNetwork;

        public EthereumProvider(NetworkType network)
        {
            SetNetwork(network);
        }
        public override async Task SendTransaction(SendTransaction tx)
        {
//            var web3 = new Web3(CurrentNetwork);
            var path = Environment.CurrentDirectory + $"\\ethereum{WalletName}.json";
            var wallet = LoadWalletFromJsonFile(path, Password);

            Account accountFrom = wallet.GetAccount(0);

            var web3 = new Web3(accountFrom, CurrentNetwork);
            var wei = Web3.Convert.ToWei(tx.Amount);
            try
            {
                var transaction =
                     await web3.TransactionManager
                        .SendTransactionAsync(
                            accountFrom.Address,
                            tx.Receiver,
                            new HexBigInteger(wei));
                var a = 1;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override List<Transaction> GetWalletHistory()
        {
            var result = new List<Transaction>();
            var path = Environment.CurrentDirectory + $"\\ethereum{WalletName}.json";
            var wallet = LoadWalletFromJsonFile(path, Password);
            var url = "https://api-ropsten.etherscan.io/api?module=account&action=txlist&address=" +  wallet.GetAccount(0).Address  + "&startblock=0&endblock=99999999&page=1&offset=10&sort=asc&apikey=YourApiKeyToken";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.PreAuthenticate = true;
            var response = request.GetResponse() as HttpWebResponse;

            var responseResult = "";
            using (var responseStream = response.GetResponseStream())
            {
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                responseResult = reader.ReadToEnd();
            }

            dynamic dynObj = JsonConvert.DeserializeObject(responseResult);
                                
            var index = 0;
            while (true)
            {
                try
                {
                    if (dynObj?.result[index] == null)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    break;
                }


                var tx =(new Transaction()
                {
                    Hash = dynObj.result[index].hash,
                    Value = dynObj.result[index].value,
                    Count = index + 1
                });

                var bigint = Web3.Convert.FromWei(System.Numerics.BigInteger.Parse(dynObj.result[index].value.ToString()));

                if (dynObj.result[index].to.ToString() == wallet.GetAccount(0).Address)
                {
                    tx.Text = $"#{index + 1}   Transaction ID: { dynObj.result[index].hash}, received coins {bigint}, confirms: { dynObj.result[index].confirmations }";
                }
                else if (dynObj.result[index].to.ToString() != wallet.GetAccount(0).Address)
                {
                    tx.Text = $"#{index + 1}   Transaction ID: { dynObj.result[index].hash}, sent coins {bigint}, confirms: { dynObj.result[index].confirmations }";
                }

                result.Add(tx);

                index++;
            }

            return result;
        }

        public override void RestoreWallet(string walletName)
        {
            throw new NotImplementedException();
        }

        public override decimal GetBalance()
        {
            var web3 = new Web3(CurrentNetwork);
            var path = Environment.CurrentDirectory + $"\\ethereum{WalletName}.json";
            var wallet = LoadWalletFromJsonFile(path, Password);
            WalletAddress = wallet.GetAccount(0).Address;
            var totalBalance = 0.0m;
            for (var i = 0; i < 20; i++)
            {
                var balance = web3.Eth.GetBalance.SendRequestAsync(wallet.GetAccount(i).Address).Result;
                var etherAmount = Web3.Convert.FromWei(balance.Value);
                totalBalance += etherAmount;
            }

            return totalBalance;
        }

        public override decimal GetUSDBalance()
        {
            var url = "https://api.coinmarketcap.com/v1/ticker/ethereum/?convert=usd";

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
                    CurrentNetwork = "https://mainnet.infura.io";
                    break;
                case NetworkType.TestNet:
                    CurrentNetwork = "https://ropsten.infura.io";
                    break;
            }
        }

        private Nethereum.HdWallet.Wallet LoadWalletFromJsonFile(string path, string pass)
        {
            string words = string.Empty;
            try
            {
                string line = File.ReadAllText(path);
                dynamic results = JsonConvert.DeserializeObject<dynamic>(line);
                string encryptedWords = results.encryptedWords;
                words = Rijndael.Decrypt(encryptedWords, pass, KeySize.Aes256);
//                string dataAndTime = results.date;
            }
            catch (Exception e)
            {
            }

            return new Nethereum.HdWallet.Wallet(words, null);
        }
    }
}
