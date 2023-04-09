﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TimeFund.DataAccess;
using TimeFund.Models;
using Activity = TimeFund.Models.Activity;

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
                AllActivities.Add(existingActivity);
            }
            else
            {
                AllActivities.Add(new UIActivity(freshActivity, totalUsage));
            }
        }
        if (!AllActivities.Any(a => a.Id == CurrentActivity.Id))
        {
            CurrentActivity = UIActivity.ZERO_ACTIVITY;
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
        else if (CurrentActivity != UIActivity.ZERO_ACTIVITY && (CurrentActivity.Multiplier >= 0 || CurrentTimeFund > TimeSpan.Zero))
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
