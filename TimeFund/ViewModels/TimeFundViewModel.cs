﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TimeFund.DataAccess;
using TimeFund.Models;

namespace TimeFund.ViewModels;

public partial class TimeFundViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimerFormat))]
    private TimeSpan currentTimeFund = new();
    public ObservableCollection<UIActivity> AllActivities { get; set; } = new();
    public IEnumerable<UIActivity> NonNegativeActivities => AllActivities.Where(a => a.Multiplier >= 0).OrderByDescending(a => a.Multiplier);
    public IEnumerable<UIActivity> NegativeActivities => AllActivities.Where(a => a.Multiplier < 0).OrderByDescending(a => a.Multiplier);
    [ObservableProperty]
    private UIActivity currentActivity = UIActivity.ZERO_ACTIVITY;
    public string TimerFormat => $"{(int)CurrentTimeFund.TotalHours:D2}:{CurrentTimeFund.Minutes:D2}:{CurrentTimeFund.Seconds:D2}";
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
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var today = DateTime.UtcNow;
        AllActivities.Clear();
        foreach (var activity in activities)
        {
            var totalUsage = await dataAccess.GetTotalUsageOverlappingIntervalForActivityAsync(activity, yesterday, today);
            var uiActivity = new UIActivity(activity, totalUsage);
            AllActivities.Add(uiActivity);
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
        else if (CurrentActivity != UIActivity.ZERO_ACTIVITY)
        {
            StartTimer();
        }
    }

    [RelayCommand]
    private void ResetTimer()
    {
        StopTimer();
        CurrentTimeFund = TimeSpan.Zero;
    }
}
