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
        public CoinProvider CoinProvider { get; set; }
    }
}
