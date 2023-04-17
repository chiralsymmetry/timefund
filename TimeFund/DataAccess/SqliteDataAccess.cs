using SQLite;
using TimeFund.Models;

namespace TimeFund.DataAccess;

public class SqliteDataAccess : IDataAccess
{
    private readonly string databasePath;
    private readonly SQLiteOpenFlags databaseFlags;
    private SQLiteAsyncConnection? database;

    public SqliteDataAccess(string databaseName = "timefund.db", SQLiteOpenFlags flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache)
    {
        databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
        databaseFlags = flags;
    }

    private async Task Init()
    {
        if (database == null)
        {
            database = new SQLiteAsyncConnection(databasePath, databaseFlags);
            await database.CreateTableAsync<Activity>().ConfigureAwait(false);
            await database.CreateTableAsync<SqliteUsageLog>().ConfigureAwait(false);
        }
    }

    public async Task<int> InsertActivityAsync(Activity activity)
    {
        await Init().ConfigureAwait(false);
        return await database!.InsertAsync(activity).ConfigureAwait(false);
    }

    public async Task<Activity?> GetActivityAsync(int id)
    {
        await Init().ConfigureAwait(false);
        return await database!.Table<Activity>().Where(a => a.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<int> UpdateActivityAsync(Activity activity)
    {
        await Init().ConfigureAwait(false);
        return await database!.UpdateAsync(activity).ConfigureAwait(false);
    }

    public async Task<int> DeleteActivityAsync(Activity activity)
    {
        await Init().ConfigureAwait(false);
        var usageLogs = await database!.Table<SqliteUsageLog>().Where(u => u.ActivityId == activity.Id).ToListAsync().ConfigureAwait(false);
        foreach (var usageLog in usageLogs)
        {
            await database.DeleteAsync(usageLog).ConfigureAwait(false);
        }
        return await database!.DeleteAsync(activity).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        await Init().ConfigureAwait(false);
        return await database!.Table<Activity>().OrderByDescending(a => a.Multiplier).ToListAsync().ConfigureAwait(false);
    }

    public async Task<int> InsertUsageLogAsync(Activity activity, DateTime end, TimeSpan duration)
    {
        await Init().ConfigureAwait(false);
        var sqliteUsageLog = new SqliteUsageLog()
        {
            Id = 0,
            ActivityId = activity.Id,
            Multiplier = activity.Multiplier,
            StartTime = end - duration,
            EndTime = end,
            Duration = duration
        };
        return await database!.InsertAsync(sqliteUsageLog).ConfigureAwait(false);
    }

    public async Task<UsageLog?> GetUsageLogAsync(int id)
    {
        await Init().ConfigureAwait(false);
        UsageLog? output = null;
        var sqliteUsageLog = await database!.Table<SqliteUsageLog>().Where(u => u.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
        if (sqliteUsageLog != null)
        {
            var activity = await GetActivityAsync(sqliteUsageLog.ActivityId).ConfigureAwait(false);
            output = new UsageLog(sqliteUsageLog.Id, activity!, sqliteUsageLog.Multiplier, sqliteUsageLog.EndTime, sqliteUsageLog.Duration);
        }
        return output;
    }

    public async Task<IEnumerable<UsageLog>> GetAllUsageLogsForActivityAsync(Activity activity)
    {
        await Init().ConfigureAwait(false);
        List<UsageLog> output = new();
        var sqliteUsageLogs = await database!.Table<SqliteUsageLog>().Where(u => u.ActivityId == activity.Id).OrderBy(u => u.StartTime).ToListAsync().ConfigureAwait(false);
        foreach (var sqliteUsageLog in sqliteUsageLogs)
        {
            var usageLog = new UsageLog(sqliteUsageLog.Id, activity, sqliteUsageLog.Multiplier, sqliteUsageLog.EndTime, sqliteUsageLog.Duration);
            output.Add(usageLog);
        }
        return output;
    }

    public async Task<IEnumerable<UsageLog>> GetAllUsageLogsOverlappingIntervalForActivityAsync(Activity activity, DateTime start, DateTime end)
    {
        await Init().ConfigureAwait(false);
        List<UsageLog> output = new();
        var sqliteUsageLogs = await database!.Table<SqliteUsageLog>().Where(u => u.ActivityId == activity.Id && u.StartTime <= end && start <= u.EndTime).OrderBy(u => u.StartTime).ToListAsync().ConfigureAwait(false);
        foreach (var sqliteUsageLog in sqliteUsageLogs)
        {
            var usageLog = new UsageLog(sqliteUsageLog.Id, activity, sqliteUsageLog.Multiplier, sqliteUsageLog.EndTime, sqliteUsageLog.Duration);
            output.Add(usageLog);
        }
        return output;
    }

    public async Task<TimeSpan> GetTotalUsageForActivityAsync(Activity activity)
    {
        await Init().ConfigureAwait(false);
        var totalUsageInSeconds = (await database!.Table<SqliteUsageLog>().Where(u => u.ActivityId == activity.Id).ToListAsync().ConfigureAwait(false)).Sum(u => u.Duration.TotalSeconds);
        return TimeSpan.FromSeconds(totalUsageInSeconds);
    }

    public async Task<TimeSpan> GetTotalUsageOverlappingIntervalForActivityAsync(Activity activity, DateTime start, DateTime end)
    {
        await Init().ConfigureAwait(false);
        var totalUsageInTicks = (await database!.Table<SqliteUsageLog>()
            .Where(u => u.ActivityId == activity.Id && u.StartTime <= end && start <= u.EndTime).ToListAsync().ConfigureAwait(false))
            .Sum(u => Math.Min(u.EndTime.Ticks, end.Ticks) - Math.Max(u.StartTime.Ticks, start.Ticks));
        return TimeSpan.FromTicks(totalUsageInTicks);
    }

    public async Task<int> UpdateUsageLogAsync(UsageLog usageLog)
    {
        await Init().ConfigureAwait(false);
        var sqliteUsageLog = new SqliteUsageLog()
        {
            Id = usageLog.Id,
            ActivityId = usageLog.Activity.Id,
            Multiplier = usageLog.Multiplier,
            StartTime = usageLog.EndTime - usageLog.Duration,
            EndTime = usageLog.EndTime,
            Duration = usageLog.Duration
        };
        return await database!.UpdateAsync(sqliteUsageLog).ConfigureAwait(false);
    }

    public async Task<int> DeleteUsageLogAsync(UsageLog usageLog)
    {
        await Init().ConfigureAwait(false);
        var sqliteUsageLog = new SqliteUsageLog()
        {
            Id = usageLog.Id,
            ActivityId = usageLog.Activity.Id,
            Multiplier = usageLog.Multiplier,
            StartTime = usageLog.EndTime - usageLog.Duration,
            EndTime = usageLog.EndTime,
            Duration = usageLog.Duration
        };
        return await database!.DeleteAsync(sqliteUsageLog).ConfigureAwait(false);
    }

    public async Task<IEnumerable<UsageLog>> GetAllUsageLogsAsync()
    {
        await Init().ConfigureAwait(false);
        List<UsageLog> output = new();
        var sqliteUsageLogs = await database!.Table<SqliteUsageLog>().OrderBy(u => u.StartTime).ToListAsync().ConfigureAwait(false);
        Dictionary<int, Activity> idToActivity = new();
        foreach (var sqliteUsageLog in sqliteUsageLogs)
        {
            if (!idToActivity.TryGetValue(sqliteUsageLog.ActivityId, out var activity))
            {
                activity = await GetActivityAsync(sqliteUsageLog.ActivityId).ConfigureAwait(false);
                idToActivity.Add(sqliteUsageLog.ActivityId, activity!);
            }
            var usageLog = new UsageLog(sqliteUsageLog.Id, activity!, sqliteUsageLog.Multiplier, sqliteUsageLog.EndTime, sqliteUsageLog.Duration);
            output.Add(usageLog);
        }
        return output;
    }

    public async Task<IEnumerable<UsageLog>> GetAllUsageLogsOverlappingIntervalAsync(DateTime start, DateTime end)
    {
        await Init().ConfigureAwait(false);
        List<UsageLog> output = new();
        var sqliteUsageLogs = await database!.Table<SqliteUsageLog>().Where(u => u.StartTime <= end && start <= u.EndTime).OrderBy(u => u.StartTime).ToListAsync().ConfigureAwait(false);
        Dictionary<int, Activity> idToActivity = new();
        foreach (var sqliteUsageLog in sqliteUsageLogs)
        {
            if (!idToActivity.TryGetValue(sqliteUsageLog.ActivityId, out var activity))
            {
                activity = await GetActivityAsync(sqliteUsageLog.ActivityId).ConfigureAwait(false);
                idToActivity.Add(sqliteUsageLog.ActivityId, activity!);
            }
            var usageLog = new UsageLog(sqliteUsageLog.Id, activity!, sqliteUsageLog.Multiplier, sqliteUsageLog.EndTime, sqliteUsageLog.Duration);
            output.Add(usageLog);
        }
        return output;
    }
}
