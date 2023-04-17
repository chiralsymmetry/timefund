using TimeFund.Models;
using Activity = TimeFund.Models.Activity;

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

    public Task<Activity?> GetActivityAsync(int id)
    {
        Activity? output = null;
        if (storedActivities.TryGetValue(id, out Activity? existingActivity))
        {
            output = new(existingActivity.Id, existingActivity.Icon, existingActivity.Title, existingActivity.Description, existingActivity.Multiplier);
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
        foreach (var usageLog in storedUsageLogs.Values.Where(ul => ul.Activity.Id == activity.Id).ToList())
        {
            storedUsageLogs.Remove(usageLog.Id);
        }
        if (storedActivities.Remove(activity.Id))
        {
            deletedRows = 1;
        }
        return Task.FromResult(deletedRows);
    }

    public Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        List<Activity> output = new();
        foreach (var item in storedActivities.Values)
        {
            output.Add(new(item.Id, item.Icon, item.Title, item.Description, item.Multiplier));
        }
        return Task.FromResult(output.OrderByDescending(a => a.Multiplier).AsEnumerable());
    }

    public Task<int> InsertUsageLogAsync(Activity activity, DateTime end, TimeSpan duration)
    {
        int insertedRows = 0;

        if (activity.Id > 0 && storedActivities.ContainsKey(activity.Id))
        {
            UsageLog usageLog = new(nextUsageLogId++, activity, activity.Multiplier, end, duration);
            storedUsageLogs.Add(usageLog.Id, usageLog);
            insertedRows = 1;
        }

        return Task.FromResult(insertedRows);
    }

    public Task<UsageLog?> GetUsageLogAsync(int id)
    {
        UsageLog? output = null;
        if (storedUsageLogs.TryGetValue(id, out UsageLog? existingUsageLog))
        {
            output = new(existingUsageLog.Id, existingUsageLog.Activity, existingUsageLog.Activity.Multiplier, existingUsageLog.EndTime, existingUsageLog.Duration);
        }
        return Task.FromResult(output);
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsForActivityAsync(Activity activity)
    {
        List<UsageLog> output = new();
        foreach (var existingUsageLog in storedUsageLogs.Values.Where(u => u.Activity.Id == activity.Id))
        {
            if (existingUsageLog.Activity.Id == activity.Id)
            {
                output.Add(new(existingUsageLog.Id, existingUsageLog.Activity, existingUsageLog.Activity.Multiplier, existingUsageLog.EndTime, existingUsageLog.Duration));
            }
        }
        return Task.FromResult(output.OrderBy(u => u.StartTime).AsEnumerable());
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsOverlappingIntervalForActivityAsync(Activity activity, DateTime start, DateTime end)
    {
        List<UsageLog> output = new();
        foreach (var existingUsageLog in storedUsageLogs.Values.Where(u => u.Activity.Id == activity.Id && u.StartTime <= end && start <= u.EndTime))
        {
            if (existingUsageLog.Activity.Id == activity.Id)
            {
                output.Add(new(existingUsageLog.Id, existingUsageLog.Activity, existingUsageLog.Activity.Multiplier, existingUsageLog.EndTime, existingUsageLog.Duration));
            }
        }
        return Task.FromResult(output.OrderBy(u => u.StartTime).AsEnumerable());
    }

    public Task<TimeSpan> GetTotalUsageForActivityAsync(Activity activity)
    {
        var totalUsageInSeconds = storedUsageLogs.Values.Where(u => u.Activity.Id == activity.Id).Sum(u => u.Duration.TotalSeconds);
        var totalUsage = TimeSpan.FromSeconds(totalUsageInSeconds);
        return Task.FromResult(totalUsage);
    }

    public Task<TimeSpan> GetTotalUsageOverlappingIntervalForActivityAsync(Activity activity, DateTime start, DateTime end)
    {
        var totalUsageInTicks = storedUsageLogs.Values
            .Where(u => u.Activity.Id == activity.Id && u.StartTime <= end && start <= u.EndTime)
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
        List<UsageLog> output = new();
        foreach (var existingUsageLog in storedUsageLogs.Values)
        {
            output.Add(new(existingUsageLog.Id, existingUsageLog.Activity, existingUsageLog.Activity.Multiplier, existingUsageLog.EndTime, existingUsageLog.Duration));
        }
        return Task.FromResult(output.OrderBy(u => u.StartTime).AsEnumerable());
    }

    public Task<IEnumerable<UsageLog>> GetAllUsageLogsOverlappingIntervalAsync(DateTime start, DateTime end)
    {
        List<UsageLog> output = new();
        foreach (var existingUsageLog in storedUsageLogs.Values.Where(u => u.StartTime <= end && start <= u.EndTime))
        {
            output.Add(new(existingUsageLog.Id, existingUsageLog.Activity, existingUsageLog.Activity.Multiplier, existingUsageLog.EndTime, existingUsageLog.Duration));
        }
        return Task.FromResult(output.OrderBy(u => u.StartTime).AsEnumerable());
    }
}
