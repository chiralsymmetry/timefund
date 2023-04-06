using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TimeFund.DataAccess;
using Activity = TimeFund.Models.Activity;

namespace TimeFund.ViewModels;

public partial class TimeFundViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimerFormat))]
    private TimeSpan timeFund = new();
    public ObservableCollection<Activity> AllActivities { get; set; } = new();
    public IEnumerable<Activity> NonNegativeActivities => AllActivities.Where(a => a.Multiplier >= 0).OrderByDescending(a => a.Multiplier);
    public IEnumerable<Activity> NegativeActivities => AllActivities.Where(a => a.Multiplier < 0).OrderByDescending(a => a.Multiplier);
    [ObservableProperty]
    private Activity currentActivity = Activity.ZERO_ACTIVITY;
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
        var activities = await dataAccess.GetAllActivitiesAsync();
        AllActivities.Clear();
        foreach (var activity in activities)
        {
            AllActivities.Add(activity);
        }
    }
}
