using CommunityToolkit.Mvvm.Input;
using TimeFund.DataAccess;
using TimeFund.Models;
using TimeFund.Views;

namespace TimeFund.ViewModels;

public class AllUsageLogsViewModel : ObservableViewModel
{
    private readonly IDataAccess dataAccess;

    private List<UsageLog> allUsageLogs = new();
    public List<UsageLog> AllUsageLogs
    {
        get => allUsageLogs;
        set
        {
            if (allUsageLogs != value)
            {
                allUsageLogs = value;
                OnPropertyChanged(nameof(AllUsageLogs));
            }
        }
    }

    private UsageLog selectedUsageLog = UsageLog.ZERO_USAGELOG;
    public UsageLog SelectedUsageLog
    {
        get => selectedUsageLog;
        set
        {
            if (selectedUsageLog != value)
            {
                selectedUsageLog = value;
                OnPropertyChanged(nameof(SelectedUsageLog));
            }
        }
    }

    private DateTime fromLocalDateTime;

    private DateTime FromLocalDateTime
    {
        get => fromLocalDateTime;
        set
        {
            if (fromLocalDateTime != value)
            {
                fromLocalDateTime = value;
                OnPropertyChanged(nameof(FromLocalDateTime));
                OnPropertyChanged(nameof(FromLocalDate));
                OnPropertyChanged(nameof(FromLocalTime));
            }
        }
    }

    public DateTime FromLocalDate
    {
        get => fromLocalDateTime.Date;
        set => FromLocalDateTime = new DateTime(value.Year, value.Month, value.Day, fromLocalDateTime.Hour, fromLocalDateTime.Minute, fromLocalDateTime.Second);
    }

    public TimeSpan FromLocalTime
    {
        get => fromLocalDateTime.TimeOfDay;
        set => FromLocalDateTime = fromLocalDateTime.Date + value;
    }

    private DateTime toLocalDateTime;

    private DateTime ToLocalDateTime
    {
        get => toLocalDateTime;
        set
        {
            if (toLocalDateTime != value)
            {
                toLocalDateTime = value;
                OnPropertyChanged(nameof(ToLocalDateTime));
                OnPropertyChanged(nameof(ToLocalDate));
                OnPropertyChanged(nameof(ToLocalTime));
            }
        }
    }

    public DateTime ToLocalDate
    {
        get => toLocalDateTime.Date;
        set => ToLocalDateTime = new DateTime(value.Year, value.Month, value.Day, toLocalDateTime.Hour, toLocalDateTime.Minute, toLocalDateTime.Second);
    }

    public TimeSpan ToLocalTime
    {
        get => toLocalDateTime.TimeOfDay;
        set => ToLocalDateTime = toLocalDateTime.Date + value;
    }

    private List<Activity> activities = new();
    public List<Activity> Activities
    {
        get => activities;
        set
        {
            if (activities != value)
            {
                activities = value;
                OnPropertyChanged(nameof(Activities));
            }
        }
    }
    public Activity SelectedActivity { get; set; }

    public AllUsageLogsViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
        activities = new() { Activity.ZERO_ACTIVITY };
        OnPropertyChanged(nameof(Activities));
        SelectedActivity = Activity.ZERO_ACTIVITY;
        Reset();
        Task.Run(async () => Activities = (await dataAccess.GetAllActivitiesAsync()).OrderBy(a => a.Multiplier).Prepend(Activity.ZERO_ACTIVITY).ToList());
    }

    private void Reset()
    {
        SelectedActivity = Activity.ZERO_ACTIVITY;
        OnPropertyChanged(nameof(SelectedActivity));
        FromLocalDateTime = DateTime.Now.AddMonths(-1);
        ToLocalDateTime = DateTime.Now;
    }

    public async Task LoadUsageLogs()
    {
        IEnumerable<UsageLog> allStoredUsageLogs;
        if (SelectedActivity == Activity.ZERO_ACTIVITY)
        {
            allStoredUsageLogs = await dataAccess.GetAllUsageLogsOverlappingIntervalAsync(FromLocalDateTime.ToUniversalTime(), ToLocalDateTime.ToUniversalTime());
        }
        else
        {
            allStoredUsageLogs = await dataAccess.GetAllUsageLogsOverlappingIntervalForActivityAsync(SelectedActivity, FromLocalDateTime.ToUniversalTime(), ToLocalDateTime.ToUniversalTime());
        }
        allUsageLogs = new(allStoredUsageLogs);
        OnPropertyChanged(nameof(AllUsageLogs));
        SelectedUsageLog = UsageLog.ZERO_USAGELOG;
    }
    public IRelayCommand LoadUsageLogsCommand => new RelayCommand(async () => await LoadUsageLogs());

    public IRelayCommand ResetFilterCommand => new RelayCommand(async () => { Reset(); await LoadUsageLogs(); });

    private async void UsageLogSelected()
    {
        if (SelectedUsageLog.Activity.Id > 0)
        {
            var usageLog = SelectedUsageLog;
            SelectedUsageLog = UsageLog.ZERO_USAGELOG;
            OnPropertyChanged(nameof(SelectedUsageLog));
            await Shell.Current.GoToAsync(nameof(SingleUsageLogPage), true, new Dictionary<string, object> { { nameof(SingleUsageLogViewModel.ExaminedUsageLog), usageLog } });
        }
    }
    public IRelayCommand UsageLogSelectedCommand => new RelayCommand(UsageLogSelected);
}
