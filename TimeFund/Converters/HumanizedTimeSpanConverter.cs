using Humanizer;
using System.Globalization;

namespace TimeFund.Converters;

public class HumanizedTimeSpanConverter : IValueConverter
{
    public static string HumanFormat(TimeSpan timeSpan)
    {
        return timeSpan.Humanize(3, maxUnit: Humanizer.Localisation.TimeUnit.Hour, minUnit: Humanizer.Localisation.TimeUnit.Second);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return HumanFormat(timeSpan);
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
