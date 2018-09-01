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
using Wallet.Core;
using Wallet.Core.CoinProviders;
using Wallet.Core.CredentialService;
using Wallet.Presentation.Commands;
using Wallet.Presentation.Model;
using Wallet.Presentation.View.Pages;

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
            switch (coin)
            {
                case "Bitcoin":
                    WalletModel.Testnet = true;
                    WalletModel.CoinProvider.Password = WalletModel.Password;
                    WalletModel.CoinProvider.WalletName = WalletModel.WalletName;
                    var btcDecimal = WalletModel.CoinProvider.GetBalance();
                    WalletModel.BTCValue = btcDecimal + " BTC";
                    WalletModel.USDValue = Math.Round(btcDecimal * WalletModel.CoinProvider.GetUSDBalance(), 4) + " USD";
                    WalletModel.Transactions = WalletModel.CoinProvider.GetWalletHistory();

                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    if (mainWindow == null) return;
                    var page = new BitcoinPage(mainWindow.Content);
                    mainWindow.Content = page;
                    break;
                case "Ethereum":
                    //todo
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
