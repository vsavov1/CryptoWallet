using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Wallet.Presentation.Converters
{
    public class IsEqualOrGreaterThanConverter : MarkupExtension, IValueConverter 
    {
        public static readonly IValueConverter Instance = new IsEqualOrGreaterThanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var intValue = (int)value;
                var compareToValue = 8;

                return intValue >= compareToValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
