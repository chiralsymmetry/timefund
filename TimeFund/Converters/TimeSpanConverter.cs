using Humanizer;
using System.Globalization;

namespace TimeFund.Converters;

public class TimeSpanConverter : IValueConverter
{
    public static string ColonFormat(TimeSpan timeSpan)
    {
        int hours = (int)timeSpan.TotalHours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        var timeSep = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
        return string.Format("{0:D2}{3}{1:D2}{3}{2:D2}", hours, minutes, seconds, timeSep);
    }

    public static string HumanFormat(TimeSpan timeSpan)
    {
        return timeSpan.Humanize(3, maxUnit: Humanizer.Localisation.TimeUnit.Hour);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return ColonFormat(timeSpan);
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
