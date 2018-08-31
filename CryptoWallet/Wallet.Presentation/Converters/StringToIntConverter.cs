using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Wallet.Presentation.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type t, object parameter, CultureInfo culture)
        {
            var i = 0;

            var s = value as string;
            if (s != null)
                int.TryParse(s, out i);

            return i;
        }

        public object ConvertBack(object value, Type t, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
