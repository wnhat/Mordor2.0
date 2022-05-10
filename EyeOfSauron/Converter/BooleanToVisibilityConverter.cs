using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace EyeOfSauron.Converter
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            Visibility output = Visibility.Visible;
            if (value is bool)
            {
                if ((bool)value)
                {
                    output = Visibility.Visible;
                }
                else
                {
                    output = Visibility.Hidden;
                }
            }
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
