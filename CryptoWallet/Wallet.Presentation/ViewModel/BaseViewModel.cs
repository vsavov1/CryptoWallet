using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
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
            var passwordErorPopUp = FindChild<DialogHost>((MainWindow)Application.Current.MainWindow, "PassWordDontMatch");
            if (account.RepeatPasswordBox.Password != Password)
            {
                passwordErorPopUp.IsOpen = true;
                return;
            }

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
            page.DataContext = this;
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

        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
