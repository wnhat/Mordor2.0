using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron.Converter
{
    public class EnumToBoolConberter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            bool output = false;
            if (value is ViewName)
            {
                switch (value)
                {
                    case ViewName.InspImageView:
                        output = false;
                        break;
                    case ViewName.ProductSelectView:
                        output = true;
                        break;
                    default: break;
                }
                return output;
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
