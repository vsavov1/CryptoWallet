using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Web3;
using Newtonsoft.Json;
using Rijndael256;

namespace Wallet.Core.CoinProviders
{
    public class EthereumProvider : CoinProvider
    {
        public string CurrentNetwork;
        public override Transaction SendTransaction(SendTransaction tx)
        {
            throw new NotImplementedException();
        }

        public override List<Transaction> GetWalletHistory()
        {
            throw new NotImplementedException();
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
            return 0m; //todo
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
                string dataAndTime = results.date;
            }
            catch (Exception e)
            {
            }

            return new Nethereum.HdWallet.Wallet(words, null);
        }
    }
}
