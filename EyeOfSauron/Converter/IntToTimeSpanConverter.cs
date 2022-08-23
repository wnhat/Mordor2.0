using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using EyeOfSauron.ViewModel;
using System.Windows.Media;


namespace EyeOfSauron.Converter
{
    public class IntToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            double output = 10;
            if (value is TimeSpan secondSpan)
            {
                output = (double)Math.Round(secondSpan.TotalSeconds, 2);
            }
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            TimeSpan output = TimeSpan.Zero;
            if(value is string secondString)
            {
                _ = double.TryParse(secondString, out double seconds);
                output = TimeSpan.FromSeconds(seconds);
            }
            return output;
        }
    }
}
