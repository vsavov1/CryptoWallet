using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.CoinProviders;

namespace Wallet.Core
{
    public class Wallet
    {
        public string mnemonicOrPrivateKey { get; set; }

        public NetworkProvider Network { get; set; }

        public Wallet()
        {
            
        }           
    }
}
