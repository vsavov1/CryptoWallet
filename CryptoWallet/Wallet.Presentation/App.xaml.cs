using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Wallet.Core.CredentialService;
using Wallet.Presentation.ViewModel;

namespace Wallet.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ICredentialService, CredentialService>();

            var walletWindowViewModel = container.Resolve<WalletViewModel>();
            var window = new MainWindow { DataContext = walletWindowViewModel };
            window.Show();
        }
    }
}
