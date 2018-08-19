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
using MaterialDesignThemes.Wpf;

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
        private void CloseError(object sender, RoutedEventArgs e)
        {
            FindChild<DialogHost>((MainWindow)Application.Current.MainWindow, "PassWordDontMatch").IsOpen = false;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            BindingOperations.GetMultiBindingExpression(CreateAccountBtn, Button.CommandParameterProperty).UpdateTarget();
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T childType))
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (!(child is FrameworkElement frameworkElement) || frameworkElement.Name != childName) continue;
                    foundChild = (T)child;
                    break;
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
