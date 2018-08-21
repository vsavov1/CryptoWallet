using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.CoinProviders;
using Wallet.Presentation.ViewModel;

namespace Wallet.Presentation.Model
{
    public class WalletModel : ObservableObject
    {
        public WalletModel()
        {
        }

        public CoinProvider CoinProvider { get; set; }
        public string WalletName { get; set; }
        public string Password { get; set; }


        public void SetProvider(CoinProvider coinProvider)
        {
            CoinProvider = coinProvider;
        }

        private decimal _btcValue;

        public decimal BTCValue
        {
            get => _btcValue;
            set
            {
                _btcValue = value;
                RaisePropertyChangedEvent("BTCValue");
            }
        }


        private bool _mainnet;

        public bool Mainnet
        {
            get => _mainnet;
            set
            {
              
                    CoinProvider?.SetNetwork(value ? NetworkType.MainNet : NetworkType.TestNet);
                    BTCValue = CoinProvider.GetBalance();
                    _mainnet = value;
                    RaisePropertyChangedEvent("Mainnet");
              
              
            }
        }

        private bool _testnet;
        public bool Testnet
        {
            get => _testnet;
            set
            {
               
                    CoinProvider?.SetNetwork(value ? NetworkType.TestNet : NetworkType.MainNet);
                    _testnet = value;
                    RaisePropertyChangedEvent("Testnet");
            }
        }

    }
}
