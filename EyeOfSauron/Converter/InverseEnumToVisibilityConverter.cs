using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron.Converter
{
    public class InverseEnumToVisibilityConverter : IValueConverter
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
                    output = Visibility.Visible;
                    break;
                case ViewName.ProductSelectView:
                case ViewName.Null:
                    output = Visibility.Collapsed;
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
