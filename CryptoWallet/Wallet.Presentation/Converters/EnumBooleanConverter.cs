using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Wallet.Presentation.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string parameterString = parameter as string;
                if (parameterString == null)
                    return DependencyProperty.UnsetValue;

                if (Enum.IsDefined(value.GetType(), value) == false)
                    return DependencyProperty.UnsetValue;

                object parameterValue = Enum.Parse(value.GetType(), parameterString);

                return parameterValue.Equals(value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }
}
