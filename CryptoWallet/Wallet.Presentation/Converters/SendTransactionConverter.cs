using System;
using System.Windows.Data;
using Wallet.Core;
using Wallet.Presentation.Model;

namespace Wallet.Presentation.Converters
{
    public class SendTransactionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null) return " ";
            try
            {
                var receiver = values[0].ToString();
                var amount = values[1].ToString();
                var message = values[2].ToString();
                var tx = new NewTransaction() { Amount = decimal.Parse(amount), Message = message, Receiver = receiver };

                return tx;
            }
            catch (Exception e)
            {
                return new NewTransaction();
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
