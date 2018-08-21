using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.CoinProviders
{
    public abstract class CoinProvider
    {
        public abstract void SendTransaction();
        public abstract void SignTransaction();
        public abstract void GetTransaction();
        public abstract void GetWalletHistory(); 
        public abstract void CreateWallet(string walletName);
        public abstract void RestoreWallet(string walletName);
        public abstract decimal GetBalance();
        public abstract void SetNetwork(NetworkType network);
        public string Mnemonic { get; set; }
        public string Password { get; set; }
        public string WalletName { get; set; }
    }
}
