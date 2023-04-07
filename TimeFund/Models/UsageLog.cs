namespace TimeFund.Models;

public class UsageLog
{
    public int Id { get; set; }
    public Activity Activity { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public UsageLog(int id, Activity activity, DateTime end, TimeSpan duration)
    {
        Id = id;
        Activity = activity;
        StartTime = end - duration;
        EndTime = end;
        Duration = duration;
    }
}
