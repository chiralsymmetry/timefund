using TimeFund.Models;

namespace TimeFund.DataAccess;

public interface IDataAccess
{
    Task<int> InsertAsync(Activity activity);
    Task<Activity?> GetActivity(int id);
    Task<int> UpdateAsync(Activity activity);
    Task<int> DeleteAsync(Activity activity);
    Task<IEnumerable<Activity>> GetAllActivitiesAsync();
}
