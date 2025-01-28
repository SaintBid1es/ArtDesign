using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArtDesign.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        // Преобразование bool в Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        // Преобразование Visibility обратно в bool (не используется)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
                return vis == Visibility.Visible;
            return false;
        }
    }
}