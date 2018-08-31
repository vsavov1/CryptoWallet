using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Wallet.Core;
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

        private string _btcValue;

        public string BTCValue
        {
            get => _btcValue;
            set
            {
                _btcValue = value;
                RaisePropertyChangedEvent("BTCValue");
            }
        }

        private string _usdValue;

        public string USDValue
        {
            get => _usdValue;
            set
            {
                _usdValue = value;
                RaisePropertyChangedEvent("USDValue");
            }
        }

        private List<Transaction> _transactions;

        public List<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                BindingOperations.EnableCollectionSynchronization(value, this);
                _transactions = value;
                RaisePropertyChangedEvent("Transactions");
            }
        }


        private bool _mainnet;

        public bool Mainnet
        {
            get => _mainnet;
            set
            {
                try
                {
                    CoinProvider?.SetNetwork(value ? NetworkType.MainNet : NetworkType.TestNet);
                    _mainnet = value;
                    var btcDecimal = CoinProvider.GetBalance();
                    BTCValue = btcDecimal + " BTC";
                    USDValue = Math.Round(btcDecimal * CoinProvider.GetUSDBalance(), 4) + " USD";
                    RaisePropertyChangedEvent("Mainnet");
                    Transactions = new List<Transaction>();
                    Transactions = CoinProvider.GetWalletHistory();
                }
                catch (Exception e)
                {
                }
            }
        }

        private bool _testnet;
        public bool Testnet
        {
            get => _testnet;
            set
            {
                try
                {
                    CoinProvider?.SetNetwork(value ? NetworkType.TestNet : NetworkType.MainNet);
                    _testnet = value;
                    var btcDecimal = CoinProvider.GetBalance();
                    BTCValue = btcDecimal + " BTC";
                    USDValue = Math.Round(btcDecimal * CoinProvider.GetUSDBalance(), 4) + " USD";
                    RaisePropertyChangedEvent("Testnet");
                    Transactions = new List<Transaction>();
                    Transactions = CoinProvider.GetWalletHistory();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
               
            }
        }

    }
}
