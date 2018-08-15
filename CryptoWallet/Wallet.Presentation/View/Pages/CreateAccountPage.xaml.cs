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
    /// Interaction logic for CreateAccountPage.xaml
    /// </summary>
    public partial class CreateAccountPage : Page
    {
        public object MainWindow { get; set; }
        public CreateAccountPage(object mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        public void BackToMainWindow(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null) mainWindow.Content = MainWindow;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var be = BindingOperations.GetMultiBindingExpression(CreateAccountBtn, Button.CommandParameterProperty);
            be.UpdateTarget();
        }
    }
}
