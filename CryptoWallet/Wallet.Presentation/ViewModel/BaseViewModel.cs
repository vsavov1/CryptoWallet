using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    public class BaseViewModel : ObservableObject
    {
        public WalletModel WalletModel { get; set; }
        public ICredentialService CredentialService { get; set; }

        public BaseViewModel(ICredentialService service)
        {
            CredentialService = service;
            WalletModel = new WalletModel();
        }

        public BaseViewModel()
        {
            WalletModel = new WalletModel();
            WalletModel.Mainnet = true;
        }

        public ICommand OpenCreateAccount => new DelegateCommand(OpenCreateAccountPage);
        public ICommand OpenLogin => new DelegateCommand(OpenLoginPage);
        public ICommand LoginCommand => new RelayCommand<Account>(Login);
        public ICommand CreateAccountCommand => new RelayCommand<NewAccount>(CreateAccount);

        private void Login(Account account)
        {
            var mnemonic = CredentialService.UnlockAccount(account.PasswordBox.Password.ToString(), account.AccountName);
            if (mnemonic != "")
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow == null) return;
                var page = new SelectCoinPage(mainWindow.Content);
                mainWindow.Content = page;
                WalletModel.WalletName = account.AccountName;
                account.PasswordBox.Clear();
            }
        }

        private void CreateAccount(NewAccount account)
        {
            CredentialService.CreateAccount(account.PasswordBox.Password.ToString(), account.AccountName);

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;
            var page = new SelectCoinPage(mainWindow.Content);
            mainWindow.Content = page;
            WalletModel.WalletName = account.AccountName;
            account.PasswordBox.Clear();
            account.RepeatPasswordBox.Clear();
        }

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

        private string _password;
    }
}
