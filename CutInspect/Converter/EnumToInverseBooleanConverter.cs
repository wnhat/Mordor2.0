using CutInspect.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CutInspect.Converter
{
    public class EnumToInverseBooleanConverter: IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }

            bool output = value switch
            {
                WorkType.DAY => false,
                WorkType.NIGHT => true,
                _ => false,
            };
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            WorkType output = value switch
            {
                true => WorkType.NIGHT,
                false => WorkType.DAY,
                _ => WorkType.NIGHT,
            };
            return output;
        }
    }
}
