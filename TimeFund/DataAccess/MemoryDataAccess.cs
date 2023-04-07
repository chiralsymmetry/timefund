using System.Linq;
using TimeFund.Models;

namespace TimeFund.DataAccess;

public class MemoryDataAccess : IDataAccess
{
    private readonly Dictionary<int, Activity> storedActivities = new();
    private readonly Dictionary<int, UsageLog> storedUsageLogs = new();
    private int nextActivityId = 1;
    private int nextUsageLogId = 1;

    public Task<int> InsertActivityAsync(Activity activity)
    {
        int insertedRows = 0;
        if (activity.Id == 0)
        {
            activity.Id = nextActivityId++;
            storedActivities.Add(activity.Id, activity);
            insertedRows = 1;
        }
        return Task.FromResult(insertedRows);
    }

    public Task<Activity?> GetActivity(int id)
    {
        if (storedActivities.TryGetValue(id, out Activity? output))
        {
        }
        return Task.FromResult(output);
    }

    public Task<int> UpdateActivityAsync(Activity activity)
    {
        int updatedRows = 0;
        if (activity.Id > 0 && storedActivities.ContainsKey(activity.Id))
        {
            if (storedActivities[activity.Id] != activity)
            {
                storedActivities[activity.Id].Icon = activity.Icon;
                storedActivities[activity.Id].Title = activity.Title;
                storedActivities[activity.Id].Description = activity.Description;
                storedActivities[activity.Id].Multiplier = activity.Multiplier;
            }
            updatedRows = 1;
        }
        return Task.FromResult(updatedRows);
    }

    public Task<int> DeleteActivityAsync(Activity activity)
    {
        int deletedRows = 0;
        if (storedActivities.Remove(activity.Id))
        {
            deletedRows = 1;
        }
        return Task.FromResult(deletedRows);
    }

    public Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        IEnumerable<Activity> output = storedActivities.Values.ToList<Activity>();
        return Task.FromResult(output);
    }

    public Task<int> InsertUsageLog(Activity activity, DateTime end, TimeSpan duration)
    {
        int insertedRows = 0;

        if (activity.Id > 0 && storedActivities.ContainsKey(activity.Id))
        {
            UsageLog usageLog = new(nextUsageLogId++, activity, end, duration);
            storedUsageLogs.Add(usageLog.Id, usageLog);
            insertedRows = 1;
        }

        return Task.FromResult(insertedRows);
    }

    public Task<UsageLog?> GetUsageLogAsync(int id)
    {
        if (storedUsageLogs.TryGetValue(id, out UsageLog? output))
        {
        }
        return Task.FromResult(output);
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsForActivityAsync(Activity activity)
    {
        IEnumerable<UsageLog> usageLogs = storedUsageLogs.Values.Where(u => u.Activity == activity).ToList();
        return Task.FromResult(usageLogs);
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsOverlappingIntervalForActivityAsync(Activity activity, DateTime start, DateTime end)
    {
        IEnumerable<UsageLog> usageLogs = storedUsageLogs.Values.Where(u => u.Activity == activity && u.StartTime <= end && start <= u.EndTime).ToList();
        return Task.FromResult(usageLogs);
    }

    public Task<TimeSpan> GetTotalUsageForActivity(Activity activity)
    {
        var totalUsageInSeconds = storedUsageLogs.Values.Where(u => u.Activity == activity).Sum(u => u.Duration.TotalSeconds);
        var totalUsage = TimeSpan.FromSeconds(totalUsageInSeconds);
        return Task.FromResult(totalUsage);
    }

    public Task<TimeSpan> GetTotalUsageOverlappingIntervalForActivityAsync(Activity activity, DateTime start, DateTime end)
    {
        var totalUsageInTicks = storedUsageLogs.Values
            .Where(u => u.Activity == activity && u.StartTime <= end && start <= u.EndTime)
            .Select(u => Math.Min(u.EndTime.Ticks, end.Ticks) - Math.Max(u.StartTime.Ticks, start.Ticks))
            .Sum();
        var totalUsage = TimeSpan.FromTicks(totalUsageInTicks);
        return Task.FromResult(totalUsage);
    }

    public Task<int> UpdateUsageLogAsync(UsageLog usageLog)
    {
        int updatedRows = 0;
        if (usageLog.Id > 0 && storedUsageLogs.ContainsKey(usageLog.Id))
        {
            if (storedUsageLogs[usageLog.Id] != usageLog)
            {
                storedUsageLogs[usageLog.Id].Activity = usageLog.Activity;
                storedUsageLogs[usageLog.Id].StartTime = usageLog.StartTime;
                storedUsageLogs[usageLog.Id].EndTime = usageLog.EndTime;
                storedUsageLogs[usageLog.Id].Duration = usageLog.Duration;
            }
            updatedRows = 1;
        }
        return Task.FromResult(updatedRows);
    }

    public Task<int> DeleteUsageLogAsync(UsageLog usageLog)
    {
        int deletedRows = 0;
        if (storedUsageLogs.Remove(usageLog.Id))
        {
            deletedRows = 1;
        }
        return Task.FromResult(deletedRows);
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsAsync()
    {
        IEnumerable<UsageLog> output = storedUsageLogs.Values.ToList();
        return Task.FromResult(output);
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsOverlappingIntervalAsync(DateTime start, DateTime end)
    {
        IEnumerable<UsageLog> usageLogs = storedUsageLogs.Values.Where(u => u.StartTime <= end && start <= u.EndTime).ToList();
        return Task.FromResult(usageLogs);
    }
}
