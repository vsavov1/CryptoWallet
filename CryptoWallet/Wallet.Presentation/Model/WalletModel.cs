using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.CoinProviders;

namespace Wallet.Presentation.Model
{
    public class WalletModel
    {
        public WalletModel()
        {
            Mainnet = true;
            Testnet = false;
        }

        public CoinProvider CoinProvider { get; set; }
        public string WalletName { get; set; }
        public bool Mainnet { get; set; }
        public bool Testnet { get; set; }

    }
}
