using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBitcoin.KeyManagement;
using NBitcoin;
using QBitNinja.Client;

namespace Wallet.Core.CoinProviders
{
    public class BitcoinProvider : CoinProvider
    {
        private string walletName { get; set; }
        private string password { get; set; }
        private string mnemonic { get; set; }
        private string address { get; set; }
        public Network CurrentNetwork { get; set; }

        public BitcoinProvider(Network network)
        {
            CurrentNetwork = network;

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

        public override void SignTransaction()
        {
            throw new NotImplementedException();
        }

        public override void GetTransaction()
        {
            throw new NotImplementedException();
        }

        public override void GetWalletHistory()
        {
            throw new NotImplementedException();
        }

        public override void CreateWallet(string walletName)
        {
            Safe.Create(out var mnemonic, password, @".\" + walletName + ".json", CurrentNetwork);
            Console.WriteLine($"Mnemonic: {mnemonic}");
//            CredentialService.CredentialService.Encrypt(password, mnemonic.ToString(), walletName);  //WTF  mnemonic.ToString()
        }

        public override void RestoreWallet(string walletName)
        {

            //            Safe.Recover(mnemonic, pw, walletFilePath + "RecoverdWallet" + rand.Next(), Network.TestNet, DateTimeOffset.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            Safe.Recover(new Mnemonic(mnemonic), password, $@".\{walletName}", global::NBitcoin.Network.TestNet);

        }

        public override void GetBalance()
        {
            Safe.Load(password, $@"{walletName}");
            var client = new QBitNinjaClient(Network.TestNet);
            decimal totalBalance = 0;
            var balance = client.GetBalance(BitcoinAddress.Create(address, CurrentNetwork), true).Result;
            foreach (var entry in balance.Operations)
            {
                foreach (var coin in entry.ReceivedCoins)
                {
                    var amount = (Money)coin.Amount;
                    var currentAmount = amount.ToDecimal(MoneyUnit.BTC);
                    totalBalance += currentAmount;
                }
            }

            Console.WriteLine($"Total balance: {totalBalance}");
        }
    }
}
