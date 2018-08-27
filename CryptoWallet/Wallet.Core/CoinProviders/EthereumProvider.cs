using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.CoinProviders
{
    public class EthereumProvider : CoinProvider
    {
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

        public override List<Transaction> GetWalletHistory()
        {
            throw new NotImplementedException();
        }

        public override void CreateWallet(string walletName)
        {
            throw new NotImplementedException();
        }

        public override void RestoreWallet(string walletName)
        {
            throw new NotImplementedException();
        }

        public override decimal GetBalance()
        {
            throw new NotImplementedException();
        }

        public override decimal GetUSDBalance()
        {
            throw new NotImplementedException();
        }

        public override void SetNetwork(NetworkType network)
        {
            throw new NotImplementedException();
        }
    }
}
