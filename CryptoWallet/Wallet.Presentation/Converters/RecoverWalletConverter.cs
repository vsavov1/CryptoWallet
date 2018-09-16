using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Wallet.Presentation.Model;
using Wallet.Presentation.ViewModel;

namespace Wallet.Presentation.Converters
{
 
    public class RecoverWalletConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null) return " ";
            try
            {
                var accountName = values[0].ToString();
                var walletName = values[1].ToString();
                var password = (values[2] as PasswordBox);
                var coin  = (bool)values[3] == false ? RecoveryCoin.Bitcoin : RecoveryCoin.Ethereum;

                var account = new RecoveryCoinModel() { MnemonicPhrase = accountName, PasswordBox = password, WalletName = walletName, RecoveryCoin = coin };

                return account;
            }
            catch (Exception e)
            {
                return new RecoveryCoinModel();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] values = null;
            if (value != null)
                return values = value.ToString().Split(' ');
            return values;
        }
    }
}
