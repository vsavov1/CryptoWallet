using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NBitcoin;
using Wallet.Core;
using Wallet.Core.CoinProviders;
using Wallet.Core.CredentialService;
using Wallet.Presentation.Commands;
using Wallet.Presentation.Model;
using Wallet.Presentation.View.Pages;
using NetworkType = Wallet.Core.CoinProviders.NetworkType;

namespace Wallet.Presentation.ViewModel
{
    public class WalletViewModel : BaseViewModel
    {
        private int _loaderProgerss;
        public int LoaderProgerss
        {
            get => _loaderProgerss;
            set
            {
                _loaderProgerss = value;
                RaisePropertyChangedEvent("LoaderProgerss");
            }
        }

        private Visibility _loaderVisibility;
        public Visibility LoaderVisibility
        {
            get => _loaderVisibility;
            set
            {
                _loaderVisibility = value;
                RaisePropertyChangedEvent("LoaderVisibility");
            }
        }


        public WalletViewModel(ICredentialService service) : base(service)
        {
            LoaderVisibility = Visibility.Hidden;
            TxStatus = Visibility.Hidden;
        }
        public WalletViewModel()
        {
        }


        public bool PopUpReceive
        {
            get => _popUpReceive;
            set
            {
                _popUpReceive = value;
                RaisePropertyChangedEvent("PopUpReceive");
            }
        }

        private bool _popUpReceive;

        public bool PopUpSend
        {
            get => _popUpSend;
            set
            {
                _popUpSend = value;
                RaisePropertyChangedEvent("PopUpSend");
            }
        }

        private Visibility _txStatus;

        public Visibility TxStatus
        {
            get => _txStatus;
            set
            {
                _txStatus = value;
                RaisePropertyChangedEvent("TxStatus");
            }
        }

        private bool _popUpSend;

        public ICommand SelectCoin => new RelayCommand<string>(SelectCoinProvider);

        private void SelectCoinProvider(string coin)
        {
            WalletModel.Testnet = true;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;

            switch (coin)
            {
                case "Bitcoin":
                    WalletModel.SetProvider(new BitcoinProvider(Network.TestNet) { WalletName = WalletModel.WalletName, Password = WalletModel.Password });
                    WalletModel.CoinProvider.Password = WalletModel.Password;
                    WalletModel.CoinProvider.WalletName = WalletModel.WalletName;
                    var btcDecimal = WalletModel.CoinProvider.GetBalance();
                    WalletModel.Value = btcDecimal + " BTC";
                    WalletModel.USDValue = Math.Round(btcDecimal * WalletModel.CoinProvider.GetUSDBalance(), 4) + " USD";
                    WalletModel.Transactions = WalletModel.CoinProvider.GetWalletHistory();
                    mainWindow.Content = new BitcoinPage(mainWindow.Content); 

                    break;
                case "Ethereum":
                    WalletModel.SetProvider(new EthereumProvider(NetworkType.TestNet) { WalletName = WalletModel.WalletName, Password = WalletModel.Password });
                    WalletModel.CoinProvider.Password = WalletModel.Password;
                    WalletModel.CoinProvider.WalletName = WalletModel.WalletName;
                    var ethDBalance = WalletModel.CoinProvider.GetBalance();
                    WalletModel.Transactions = WalletModel.CoinProvider.GetWalletHistory();
                    WalletModel.Value = ethDBalance + " ETH";
                    WalletModel.USDValue = Math.Round(ethDBalance * WalletModel.CoinProvider.GetUSDBalance(), 4) + " USD";
                    //                    WalletModel.Transactions = WalletModel.CoinProvider.GetWalletHistory();    //todo

                    mainWindow.Content = new EthereumPage(mainWindow.Content);

                    break;
            }
        }

        public ICommand SendTransaction => new RelayCommand<NewTransaction>(SendTransactionToCoinProvider);

        private void SendTransactionToCoinProvider(NewTransaction tx)
        {
            var txResult = WalletModel.CoinProvider.SendTransaction(tx);
//            if (txResult != null)
//            {
//                this.WalletModel.Transactions.Add(new Transaction() { Text = txResult.Text, Value = tx.Amount, Hash = txResult.Hash, Count = this.WalletModel.Transactions.Count });
//            }
            TxStatus = Visibility.Visible;

        }

        public ICommand RefreshHistory => new DelegateCommand(ExecuteRefreshHistory);

        private void ExecuteRefreshHistory()
        {
            this.WalletModel.Transactions = this.WalletModel.CoinProvider.GetWalletHistory();
        }
    }
}
