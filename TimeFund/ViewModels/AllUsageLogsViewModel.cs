using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimeFund.DataAccess;
using TimeFund.Models;

namespace TimeFund.ViewModels;

public class AllUsageLogsViewModel : ObservableViewModel
{
    private readonly IDataAccess dataAccess;

    public ObservableCollection<UsageLog> AllUsageLogs { get; set; }

    private UsageLog selectedUsageLog = UsageLog.ZERO_USAGELOG;
    public UsageLog SelectedUsageLog
    {
        get
        {
            return selectedUsageLog;
        }
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

    public List<Activity> Activities { get; }
    public Activity SelectedActivity { get; set; }

    public AllUsageLogsViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
        AllUsageLogs = new();
        Activities = Task.Run(dataAccess.GetAllActivitiesAsync).Result.OrderBy(a => a.Multiplier).ToList();
        Activities.Insert(0, Activity.ZERO_ACTIVITY);
        SelectedActivity = Activity.ZERO_ACTIVITY;
        Reset();
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
        AllUsageLogs.Clear();
        foreach (var storedUsageLog in allStoredUsageLogs)
        {
            AllUsageLogs.Add(storedUsageLog);
        }
        SelectedUsageLog = UsageLog.ZERO_USAGELOG;
    }
    private RelayCommand? loadUsageLogsCommand;
    public IRelayCommand LoadUsageLogsCommand => loadUsageLogsCommand ??= new RelayCommand(async () => await LoadUsageLogs());

    private RelayCommand? resetFilterCommand;
    public IRelayCommand ResetFilterCommand => resetFilterCommand ??= new RelayCommand(async () => { Reset(); await LoadUsageLogs(); });
}
