using CommunityToolkit.Mvvm.Input;
using Humanizer;
using System.Collections.ObjectModel;
using TimeFund.DataAccess;
using TimeFund.Models;

namespace TimeFund.ViewModels;

[QueryProperty(nameof(ExaminedActivity), nameof(ExaminedActivity))]
public class SingleActivityViewModel : ObservableViewModel
{
    private readonly IDataAccess dataAccess;

    private Activity examinedActivity = Activity.ZERO_ACTIVITY;
    public Activity ExaminedActivity
    {
        get
        {
            return examinedActivity;
        }
        set
        {
            if (examinedActivity != value)
            {
                examinedActivity = value;
                OnPropertyChanged(nameof(ExaminedActivity));
                if (examinedActivity == Activity.ZERO_ACTIVITY)
                {
                    UsageLogs.Clear();
                    OnPropertyChanged(nameof(UsageLogs));
                    TotalUsage = TimeSpan.Zero;
                }
                else
                {
                    Task.Run(async () =>
                    {
                        var usageLogs = await dataAccess.GetAllUsageLogsForActivityAsync(ExaminedActivity);
                        UsageLogs = new(usageLogs);
                        OnPropertyChanged(nameof(UsageLogs));
                        TotalUsage = await dataAccess.GetTotalUsageForActivityAsync(ExaminedActivity);
                    });
                }
            }
        }
    }

    public string MultiplierString
    {
        get
        {
            string output;
            var multiplier = ExaminedActivity.Multiplier;
            var oneHour = TimeSpan.FromHours(1);
            if (multiplier == 0)
            {
                output = "At this rate, the counter will never change.";
            }
            else
            {
                var example = "At this rate, you will save up one hour";
                if (multiplier < 0)
                {
                    multiplier *= -1;
                    example = "At this rate, you will use up one hour";
                }
                var hours = (int)(oneHour.TotalHours / multiplier);
                var minutes = (int)((oneHour.TotalMinutes / multiplier) - (60 * hours));
                var seconds = (int)((oneHour.TotalSeconds / multiplier) - (60 * minutes) - (3600 * hours));
                var hoursString = hours > 0 ? $"{hours} hour{(hours != 1 ? "s" : "")}" : "";
                var minutesString = minutes > 0 ? $"{minutes} minute{(minutes != 1 ? "s" : "")}" : "";
                var secondsString = seconds > 0 ? $"{seconds} second{(seconds != 1 ? "s" : "")}" : "";
                var hoursMinutesSeparator = "";
                var minutesSecondsSeparator = (minutes > 0 && seconds > 0) ? " and " : "";
                if (hours > 0)
                {
                    if (minutes > 0 && seconds > 0)
                    {
                        hoursMinutesSeparator = ", ";
                    }
                    else if (minutes > 0 || seconds > 0)
                    {
                        hoursMinutesSeparator = " and ";
                    }
                }
                if (hours == 0 && minutes == 0 && seconds == 0)
                {
                    output = $"{example} nearly instantaneously.";
                }
                else if (hours < 0 || minutes < 0 || seconds < 0)
                {
                    output = $"{example} in several years.";
                }
                else
                {
                    output = $"{example} in {hoursString}{hoursMinutesSeparator}{minutesString}{minutesSecondsSeparator}{secondsString}.";
                }
            }
            return output;
        }
    }

    public ObservableCollection<UsageLog> UsageLogs { get; set; } = new();

    private TimeSpan totalUsage = TimeSpan.Zero;
    public TimeSpan TotalUsage
    {
        get { return totalUsage; }
        set
        {
            if (totalUsage != value)
            {
                totalUsage = value;
                OnPropertyChanged(nameof(TotalUsage));
            }
        }
    }

    public SingleActivityViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    private void UpdateActivity()
    {
        if (ExaminedActivity != Activity.ZERO_ACTIVITY)
        {
            if (ExaminedActivity.Id > 0)
            {
                dataAccess.UpdateActivityAsync(ExaminedActivity);
            }
            else
            {
                dataAccess.InsertActivityAsync(ExaminedActivity);
            }
            Shell.Current.Navigation.PopAsync();
        }
    }
    public IRelayCommand UpdateActivityCommand => new RelayCommand(UpdateActivity);

    private async void DeleteActivity()
    {
        if (ExaminedActivity != Activity.ZERO_ACTIVITY)
        {
            var response = true;
            if (ExaminedActivity.Id > 0)
            {
                response = await Shell.Current.DisplayAlert(
                    "Delete Activity",
                    $"Do you really want to delete the activity \"{ExaminedActivity.Title}\"? It has {TotalUsage.Humanize()} of log time.",
                    "Delete", "Cancel");
            }
            if (response)
            {
                var activity = ExaminedActivity;
                ExaminedActivity = Activity.ZERO_ACTIVITY;
                await dataAccess.DeleteActivityAsync(activity);
                await Shell.Current.Navigation.PopAsync();
            }
        }
    }
    public IRelayCommand DeleteActivityCommand => new RelayCommand(DeleteActivity);
}
