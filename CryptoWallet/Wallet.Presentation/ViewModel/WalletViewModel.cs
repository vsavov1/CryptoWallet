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
        public WalletModel WalletModel { get; set; }

        public WalletViewModel(ICredentialService service) : base(service)
        {
            WalletModel = new WalletModel();
        }
        public WalletViewModel()
        {
            WalletModel = new WalletModel();
        }

        public ICommand SelectCoin => new RelayCommand<string>(SelectCoinProvider);

        private void SelectCoinProvider(string coin)
        {
            switch (coin)
            {
                case "Bitcoin":
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    if (mainWindow == null) return;
                    var page = new BitcoinPage(mainWindow.Content);
                    mainWindow.Content = page;
                    WalletModel.CoinProvider = new BitcoinProvider(Network.TestNet); //todo
                    break;
                case "Ethereum":
                    //todo
                    break;
            }
        }
    }
}
