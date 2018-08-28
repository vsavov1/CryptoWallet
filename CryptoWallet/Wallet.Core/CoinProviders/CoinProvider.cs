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
        public abstract List<Transaction> GetWalletHistory(); 
        public abstract void RestoreWallet(string walletName);
        public abstract decimal GetBalance();
        public abstract decimal GetUSDBalance();
        public abstract void SetNetwork(NetworkType network);
        public string Mnemonic { get; set; }
        public string Password { get; set; }
        public string WalletName { get; set; }
        public string WalletAddress { get; set; }
    }
}
