using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using EyeOfSauron.ViewModel;
using System.Windows.Media;


namespace EyeOfSauron.Converter
{
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }

            var output = value switch
            {
                ControlTableItem.ProductMission => 0,
                ControlTableItem.ExamMission => 1,
                _ => 0,
            };
            return output;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotImplementedException();
            }
            var output = value switch
            {
                0 => ControlTableItem.ProductMission,
                1 => ControlTableItem.ExamMission,
                _ => ControlTableItem.ProductMission,
            };
            return output;
        }
    }
}
