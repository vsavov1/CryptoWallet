using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NBitcoin;
using Wallet.Core.CoinProviders;
using Wallet.Presentation.Commands;
using Wallet.Presentation.Model;
using Wallet.Presentation.View.Pages;

namespace Wallet.Presentation.ViewModel
{
    public class WalletViewModel : BaseViewModel
    {
        public WalletModel WalletModel { get; set; }

        public WalletViewModel()
        {
            WalletModel = new WalletModel();
        }

        public ICommand SelectCoin => new RelayCommand<string>(SelectCoinProvider);
        public ICommand CreateAccount => new DelegateCommand(OpenCreateAccountPage);
        public ICommand Login => new DelegateCommand(OpenLoginPage);

        private void OpenCreateAccountPage()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;
            var page = new CreateAccountPage(mainWindow.Content);
            mainWindow.Content = page;
        }

        private void OpenLoginPage()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;
            var page = new LoginPage(mainWindow.Content);
            mainWindow.Content = page;
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChangedEvent("Password");
            }
        }

        public string Account
        {
            get => _account;
            set
            {
                _account = value;
                RaisePropertyChangedEvent("Account");
            }
        }

        private string _password;
        private string _account;

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void SelectCoinProvider(string coin)
        {
            switch (coin)
            {
                case "Bitcoin":
                    WalletModel.CoinProvider = new BitcoinProvider(Network.TestNet); //todo
                    break;
                case "Ethereum":
                    //todo
                    break;
            }
        }
    }
}
