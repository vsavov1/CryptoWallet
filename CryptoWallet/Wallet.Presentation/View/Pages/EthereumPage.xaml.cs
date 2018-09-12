using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wallet.Presentation.ViewModel;

namespace Wallet.Presentation.View.Pages
{
    /// <summary>
    /// Interaction logic for EthereumPage.xaml
    /// </summary>
    public partial class EthereumPage : Page
    {
        public object MainWindow { get; set; }
        public EthereumPage(object mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;
        }

        public void BackToMainWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Content = MainWindow;
            }
        }

        public void Hyperlink_RequestNavigate(object sender, RoutedEventArgs e)
        {
            Process.Start("chrome.exe", "https://ropsten.etherscan.io/tx/" + ((Hyperlink)sender).NavigateUri);      //todo
        }

        private void ClosePopUpReceive(object sender, RoutedEventArgs e)
        {
            var vm = (WalletViewModel)this.DataContext;
            vm.PopUpReceive = false;
        }

        private void ClosePopUpSend(object sender, RoutedEventArgs e)
        {
            var vm = (WalletViewModel)this.DataContext;
            vm.PopUpSend = false;
            vm.TxStatus = Visibility.Hidden;
        }

        private void OpenPopUpReceive(object sender, RoutedEventArgs e)
        {
            var vm = (WalletViewModel)this.DataContext;
            vm.PopUpReceive = true;
        }

        private void OpenPopUpSend(object sender, RoutedEventArgs e)
        {
            var vm = (WalletViewModel)this.DataContext;
            vm.PopUpSend = true;
        }
    }
}
