using CutInspect.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CutInspect.Converter
{
    public class IntToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            SolidColorBrush output = value switch
            {
                1 => new SolidColorBrush(Colors.CadetBlue),
                0 => new SolidColorBrush(Colors.LightGray),
                _ => new SolidColorBrush(Colors.WhiteSmoke),
            };
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
