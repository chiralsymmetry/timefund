using System.Diagnostics;
using TimeFund.DataAccess;
using TimeFund.Models;
using Activity = TimeFund.Models.Activity;

namespace TimeFund.ViewModels;

public class TimeFundViewModel
{
    public TimeSpan TimeFund { get; set; } = new();
    public IEnumerable<Activity> AllActivities { get; set; } = Enumerable.Empty<Activity>();
    public IEnumerable<Activity> NonNegativeActivities => AllActivities.Where(a => a.Multiplier >= 0);
    public IEnumerable<Activity> NegativeActivities => AllActivities.Where(a => a.Multiplier < 0);
    public Activity CurrentActivity { get; set; } = Activity.ZERO_ACTIVITY;
    public string TimerFormat => $"{(int)TimeFund.TotalHours:D2}:{TimeFund.Minutes:D2}:{TimeFund.Seconds:D2}";

    private readonly Stopwatch stopwatch = new();
    private readonly IDataAccess dataAccess;

    public TimeFundViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
        Task.Run(LoadActivities);
    }

    private async Task LoadActivities()
    {
        AllActivities = await dataAccess.GetAllActivitiesAsync();
    }
}
