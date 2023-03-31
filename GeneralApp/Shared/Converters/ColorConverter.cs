using System.Globalization;

namespace GeneralApp.Shared.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return null;

            var color = value.ToString();

            return Color.FromArgb(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorHex = (Color)value;
            return colorHex.ToArgbHex();
        }
    }
}
