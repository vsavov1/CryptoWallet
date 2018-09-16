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
        public string Coin { get; set; }


        public void SetProvider(CoinProvider coinProvider)
        {
            CoinProvider = coinProvider;
        }

        private string _value;

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChangedEvent("Value");
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
//                BindingOperations.EnableCollectionSynchronization(value, this);
                _transactions = value;
                RaisePropertyChangedEvent("Transactions");
            }
        }

        private NetworkType _network;
        public NetworkType Network
        {
            get => _network;
            set
            {
                try
                {
                    CoinProvider?.SetNetwork(value == NetworkType.TestNet ? NetworkType.TestNet : NetworkType.MainNet);
                    _network = value;

                    if (CoinProvider.WalletName == null)
                    {
                        return;
                    }
                    var btcDecimal = CoinProvider.GetBalance();
                    Value = btcDecimal + $" {Coin}";
                    USDValue = Math.Round(btcDecimal * CoinProvider.GetUSDBalance(), 4) + " USD";
                    RaisePropertyChangedEvent("Network");
                    Transactions = new List<Transaction>();
                    Transactions = CoinProvider.GetWalletHistory();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }

        private RecoveryCoin _recoveryCoin;
        public RecoveryCoin RecoveryCoin
        {
            get => _recoveryCoin;
            set
            {
                try
                {
                    //                    CoinProvider?.SetNetwork(value == NetworkType.TestNet ? NetworkType.TestNet : NetworkType.MainNet);
                    _recoveryCoin = value;
//                    var btcDecimal = CoinProvider.GetBalance();
//                    Value = btcDecimal + $" {Coin}";
//                    USDValue = Math.Round(btcDecimal * CoinProvider.GetUSDBalance(), 4) + " USD";
                    RaisePropertyChangedEvent("RecoveryCoin");
//                    Transactions = new List<Transaction>();
//                    Transactions = CoinProvider.GetWalletHistory();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }

    }
}
