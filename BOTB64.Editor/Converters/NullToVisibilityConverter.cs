using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BOTB64.Editor.Converters
{
    // Used to show a placeholder hint TextBlock only while no file is loaded.
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
