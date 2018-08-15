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
            if (mainWindow != null) mainWindow.Content = MainWindow;
        }
    }
}
