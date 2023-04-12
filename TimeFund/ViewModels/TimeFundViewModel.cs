using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TimeFund.DataAccess;
using TimeFund.Models;

namespace TimeFund.ViewModels;

public class TimeFundViewModel : ObservableViewModel
{
    private TimeSpan currentTimeFund = new(Preferences.Get(nameof(CurrentTimeFund), defaultValue: 0L));
    public TimeSpan CurrentTimeFund
    {
        get { return currentTimeFund; }
        set
        {
            if (currentTimeFund != value)
            {
                currentTimeFund = value;
                OnPropertyChanged(nameof(CurrentTimeFund));
                OnPropertyChanged(nameof(TimerFormat));
                Preferences.Set(nameof(CurrentTimeFund), value.Ticks);
            }
        }
    }
    private ObservableCollection<UIActivity> allActivities = new();
    public ObservableCollection<UIActivity> AllActivities
    {
        get => allActivities;
        set
        {
            if (allActivities != value)
            {
                allActivities = value;
                OnPropertyChanged(nameof(AllActivities));
                OnPropertyChanged(nameof(NonNegativeActivities));
                OnPropertyChanged(nameof(NegativeActivities));
            }
        }
    }
    public IEnumerable<UIActivity> NonNegativeActivities => AllActivities.Where(a => a.Multiplier >= 0);
    public IEnumerable<UIActivity> NegativeActivities => AllActivities.Where(a => a.Multiplier < 0);

    private UIActivity currentActivity = UIActivity.ZERO_UIACTIVITY;
    public UIActivity CurrentActivity
    {
        get { return currentActivity; }
        set
        {
            if (currentActivity != value)
            {
                currentActivity = value;
                OnPropertyChanged(nameof(CurrentActivity));
            }
        }
    }

    public string TimerFormat => $"{(int)CurrentTimeFund.TotalHours:D2}:{CurrentTimeFund.Minutes:D2}:{CurrentTimeFund.Seconds:D2}";

    private string timerButtonText = "Start";
    public string TimerButtonText
    {
        get { return timerButtonText; }
        set
        {
            if (timerButtonText != value)
            {
                timerButtonText = value;
                OnPropertyChanged(nameof(TimerButtonText));
            }
        }
    }

    private readonly Stopwatch stopwatch = new();
    private readonly IDataAccess dataAccess;

    private CancellationTokenSource TimerCancellationTokenSource { get; set; } = new();

    public TimeFundViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    public async Task LoadActivities()
    {
        // TODO: Consider caching for icon images.
        // Recreating the image XAML element is a bit costly (but not much).
        var idToExistingActivities = AllActivities.ToDictionary(a => a.Id);
        var freshActivities = (await dataAccess.GetAllActivitiesAsync()).ToList();
        AllActivities.Clear();
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var today = DateTime.UtcNow;
        var allUIActivities = new List<UIActivity>();
        foreach (var freshActivity in freshActivities)
        {
            var totalUsage = await dataAccess.GetTotalUsageOverlappingIntervalForActivityAsync(freshActivity, yesterday, today);
            if (idToExistingActivities.TryGetValue(freshActivity.Id, out var existingActivity))
            {
                existingActivity.Icon = freshActivity.Icon;
                existingActivity.Title = freshActivity.Title;
                existingActivity.Description = freshActivity.Description;
                existingActivity.Multiplier = freshActivity.Multiplier;
                existingActivity.Usage = totalUsage;
                // TODO: Check if timer ongoing for this activity, and compensate for usage not yet stored in database.
                allUIActivities.Add(existingActivity);
            }
            else
            {
                allUIActivities.Add(new UIActivity(freshActivity, totalUsage));
            }
        }
        AllActivities = new(allUIActivities);
        if (!AllActivities.Any(a => a.Id == CurrentActivity.Id))
        {
            CurrentActivity = UIActivity.ZERO_UIACTIVITY;
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
                CurrentActivity.Usage += TimeSpan.FromSeconds(stopwatch.Elapsed.TotalSeconds);
                CurrentTimeFund += TimeSpan.FromSeconds(stopwatch.Elapsed.TotalSeconds * CurrentActivity.Multiplier);
                if (CurrentActivity.Multiplier < 0 && CurrentTimeFund <= TimeSpan.Zero)
                {
                    // TODO: Play alarm sound.
                    TimerCancellationTokenSource.Cancel();
                    CurrentTimeFund = TimeSpan.Zero;
                }
            }
            stopwatch.Stop();
            // TODO: Log usage.
        });
    }

    private void StopTimer()
    {
        TimerCancellationTokenSource.Cancel();
        stopwatch.Stop();
        TimerButtonText = "Start";
    }
    private RelayCommand? stopTimerCommand;
    public IRelayCommand StopTimerCommand => stopTimerCommand ??= new RelayCommand(StopTimer);

    private void ToggleTimer()
    {
        if (stopwatch.IsRunning)
        {
            StopTimer();
        }
        else if (CurrentActivity != UIActivity.ZERO_UIACTIVITY && (CurrentActivity.Multiplier >= 0 || CurrentTimeFund > TimeSpan.Zero))
        {
            StartTimer();
        }
    }
    private RelayCommand? toggleTimerCommand;
    public IRelayCommand ToggleTimerCommand => toggleTimerCommand ??= new RelayCommand(ToggleTimer);

    private void ResetTimer()
    {
        StopTimer();
        CurrentTimeFund = TimeSpan.Zero;
    }
    private RelayCommand? resetTimerCommand;
    public IRelayCommand ResetTimerCommand => resetTimerCommand ??= new RelayCommand(ResetTimer);
}
