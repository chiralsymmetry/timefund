using TimeFund.Models;

namespace TimeFund.DataAccess;

public interface IDataAccess
{
    Task<int> InsertActivityAsync(Activity activity);
    Task<Activity?> GetActivity(int id);
    Task<int> UpdateActivityAsync(Activity activity);
    Task<int> DeleteActivityAsync(Activity activity);
    Task<IEnumerable<Activity>> GetAllActivitiesAsync();

    Task<int> InsertUsageLog(Activity activity, DateTime end, TimeSpan duration);
    Task<UsageLog?> GetUsageLogAsync(int id);
    Task<IEnumerable<UsageLog>> GetAllUsageLogsForActivityAsync(Activity activity);
    Task<IEnumerable<UsageLog>> GetAllUsageLogsForActivityInIntervalAsync(Activity activity, DateTime start, DateTime end);
    Task<TimeSpan> GetTotalUsageForActivity(Activity activity);
    Task<int> UpdateUsageLogAsync(UsageLog usageLog);
    Task<int> DeleteUsageLogAsync(UsageLog usageLog);
    Task<IEnumerable<UsageLog>> GetAllUsageLogsAsync();
    Task<IEnumerable<UsageLog>> GetAllUsageLogsInIntervalAsync(DateTime start, DateTime end);
}
