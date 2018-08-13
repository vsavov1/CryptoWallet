using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NBitcoin;
using Wallet.Core.CoinProviders;
using Wallet.Presentation.Commands;
using Wallet.Presentation.Model;

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
