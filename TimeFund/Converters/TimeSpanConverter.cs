using System.Globalization;

namespace TimeFund.Converters;

public class TimeSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            int hours = (int)timeSpan.TotalHours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
