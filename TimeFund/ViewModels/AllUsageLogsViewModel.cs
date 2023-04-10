using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TimeFund.DataAccess;
using TimeFund.Models;

namespace TimeFund.ViewModels;

public partial class AllUsageLogsViewModel : ObservableObject
{
    private readonly IDataAccess dataAccess;

    public ObservableCollection<UsageLog> AllUsageLogs { get; set; }

    [ObservableProperty]
    private UsageLog selectedUsageLog = UsageLog.ZERO_USAGELOG;

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
