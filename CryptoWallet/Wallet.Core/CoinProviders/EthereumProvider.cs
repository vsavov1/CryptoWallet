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

        public override void GetWalletHistory()
        {
            throw new NotImplementedException();
        }

        public override void CreateWallet()
        {
            throw new NotImplementedException();
        }

        public override void RestoreWallet()
        {
            throw new NotImplementedException();
        }

        public override void GetBalance()
        {
            throw new NotImplementedException();
        }
    }
}
