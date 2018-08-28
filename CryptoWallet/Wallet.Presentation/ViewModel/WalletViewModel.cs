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
        }
        public WalletViewModel()
        {
        }

        public ICommand SelectCoin => new RelayCommand<string>(SelectCoinProvider);

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

        private void SelectCoinProvider(string coin)
        {
            switch (coin)
            {
                case "Bitcoin":
                    WalletModel.CoinProvider.Password = WalletModel.Password;
                    WalletModel.CoinProvider.WalletName = WalletModel.WalletName;
                    var btcDecimal = WalletModel.CoinProvider.GetBalance();
                    WalletModel.BTCValue = btcDecimal  + " BTC";
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
    }
}
