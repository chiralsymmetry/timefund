using TimeFund.Models;

namespace TimeFund.ViewModels;

public class TimeFundViewModel
{
    public TimeSpan TimeFund { get; set; } = TimeSpan.Zero;
    public IEnumerable<Activity> AllActivities { get; set; } = Enumerable.Empty<Activity>();
    public IEnumerable<Activity> NonNegativeActivities => AllActivities.Where(a => a.Multiplier >= 0);
    public IEnumerable<Activity> NegativeActivities => AllActivities.Where(a => a.Multiplier < 0);
    public Activity CurrentActivity { get; set; } = Activity.ZERO_ACTIVITY;
}
