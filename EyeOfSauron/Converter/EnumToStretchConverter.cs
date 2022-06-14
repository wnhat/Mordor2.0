using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using EyeOfSauron.ViewModel;
using System.Windows.Media;


namespace EyeOfSauron.Converter
{
    public class EnumToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }

            var output = value switch
            {
                ViewName.InspImageView or ViewName.Null => Stretch.Fill,
                ViewName.ProductSelectView => Stretch.None,
                _ => Stretch.None,
            };
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
