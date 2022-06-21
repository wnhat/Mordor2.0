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
            switch (value)
            {
                case ViewName.InspImageView:
                case ViewName.Null:
                    output = Visibility.Collapsed;
                    break;
                case ViewName.ProductSelectView:
                    output = Visibility.Visible;
                    break;
                default:
                    output = Visibility.Collapsed;
                    break;
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
