using System;
using System.Collections.Generic;
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
    /// Interaction logic for BitcoinPage.xaml
    /// </summary>
    public partial class BitcoinPage : Page
    {
        public object MainWindow { get; set; }
        public BitcoinPage(object mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        public void BackToMainWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Content = MainWindow;
            }
        }
    }
}
