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
    public class IntToResultStringConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            string output = value switch
            {
                1 => JudgeResult.OK.ToString(),
                0 => JudgeResult.NG.ToString(),
                _ => "Undefined",
            };
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
