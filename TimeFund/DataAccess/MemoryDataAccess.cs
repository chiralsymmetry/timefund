using TimeFund.Models;

namespace TimeFund.DataAccess;

public class MemoryDataAccess : IDataAccess
{
    private readonly Dictionary<int, Activity> activities = new();
    private int nextId = 1;

    public Task<int> InsertAsync(Activity activity)
    {
        int insertedRows = 0;
        if (activity.Id == 0)
        {
            activity.Id = nextId++;
            activities.Add(activity.Id, activity);
            insertedRows = 1;
        }
        return Task.FromResult(insertedRows);
    }

    public Task<Activity?> GetActivity(int id)
    {
        if (activities.TryGetValue(id, out Activity? output))
        {
        }
        return Task.FromResult(output);
    }

    public Task<int> UpdateAsync(Activity activity)
    {
        int updatedRows = 0;
        if (activity.Id > 0 && activities.ContainsKey(activity.Id))
        {
            if (activities[activity.Id] != activity)
            {
                activities[activity.Id].Icon = activity.Icon;
                activities[activity.Id].Title = activity.Title;
                activities[activity.Id].Description = activity.Description;
                activities[activity.Id].Multiplier = activity.Multiplier;
            }
            updatedRows = 1;
        }
        return Task.FromResult(updatedRows);
    }

    public Task<int> DeleteAsync(Activity activity)
    {
        int deletedRows = 0;
        if (activities.Remove(activity.Id))
        {
            deletedRows = 1;
        }
        return Task.FromResult(deletedRows);
    }

    public Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        IEnumerable<Activity> output = activities.Values.ToList<Activity>();
        return Task.FromResult(output);
    }
}
