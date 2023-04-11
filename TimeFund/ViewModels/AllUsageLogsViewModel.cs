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

    public AllUsageLogsViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
        AllUsageLogs = new();
    }

    public async Task LoadUsageLogs()
    {
        var allStoredUsageLogs = await dataAccess.GetAllUsageLogsAsync();
        AllUsageLogs.Clear();
        foreach (var storedUsageLog in allStoredUsageLogs)
        {
            AllUsageLogs.Add(storedUsageLog);
        }
        SelectedUsageLog = UsageLog.ZERO_USAGELOG;
    }
}
