using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron.Converter
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            Visibility output;
            if (value is double)
            {
                output = (double)value == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                output = value switch
                {
                    ViewName.InspImageView or ViewName.Null => Visibility.Collapsed,
                    ViewName.ProductSelectView => Visibility.Visible,
                    _ => Visibility.Collapsed,
                };
            }
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            return ViewName.Null;
        }
    }
}
