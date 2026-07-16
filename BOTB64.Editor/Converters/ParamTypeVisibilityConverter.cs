using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BOTB64.Runtime;

namespace BOTB64.Editor.Converters
{
    // Usage: Visibility="{Binding Type, Converter={StaticResource ParamTypeVisibility}, ConverterParameter=Integer}"
    public class ParamTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ParameterType current || parameter is not string paramName)
                return Visibility.Collapsed;

            var wanted = (ParameterType)Enum.Parse(typeof(ParameterType), paramName);
            return current == wanted ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
