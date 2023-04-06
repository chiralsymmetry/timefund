using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
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
    [ObservableProperty]
    private string timerButtonText = "Start";

    private readonly Stopwatch stopwatch = new();
    private readonly IDataAccess dataAccess;
    private CancellationTokenSource TimerCancellationTokenSource { get; set; } = new();

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

    private void StartTimer()
    {
        TimerCancellationTokenSource = new();
        stopwatch.Restart();
        Task.Run(async () =>
        {
            TimerButtonText = "Stop";
            while (!TimerCancellationTokenSource.Token.IsCancellationRequested)
            {
                stopwatch.Restart();
                await Task.Delay(1000, TimerCancellationTokenSource.Token);
                // Waiting 1000 ms... or less, if externally cancelled.
                TimeFund += TimeSpan.FromSeconds(stopwatch.Elapsed.TotalSeconds * CurrentActivity.Multiplier);
                if (CurrentActivity.Multiplier < 0 && TimeFund <= TimeSpan.Zero)
                {
                    // TODO: Play alarm sound.
                    TimerCancellationTokenSource.Cancel();
                    TimeFund = TimeSpan.Zero;
                }
            }
            stopwatch.Stop();
        });
    }

    [RelayCommand]
    private void StopTimer()
    {
        TimerCancellationTokenSource.Cancel();
        stopwatch.Stop();
        TimerButtonText = "Start";
    }

    [RelayCommand]
    private void ToggleTimer()
    {
        if (stopwatch.IsRunning)
        {
            StopTimer();
        }
        else if (CurrentActivity != Activity.ZERO_ACTIVITY)
        {
            StartTimer();
        }
    }

    [RelayCommand]
    private void ResetTimer()
    {
        StopTimer();
        TimeFund = TimeSpan.Zero;
    }
}
