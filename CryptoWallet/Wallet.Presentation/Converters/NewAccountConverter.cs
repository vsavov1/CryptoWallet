using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Wallet.Presentation.Model;

namespace Wallet.Presentation.Converters
{
    public class NewAccountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null) return " ";
            try
            {
                var accountName = values[0].ToString();
                var password = (values[1] as PasswordBox).Password.ToString();
                var repeatPassword = (values[2] as PasswordBox).Password.ToString();

                return new NewAccount() { AccountName = accountName, Password = password , RepeatPassword = repeatPassword };
            }
            catch (Exception e)
            {
                return new NewAccount();
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
