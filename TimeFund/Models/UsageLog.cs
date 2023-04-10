namespace TimeFund.Models;

public class UsageLog
{
    public static readonly UsageLog ZERO_USAGELOG = new(0, Activity.ZERO_ACTIVITY, DateTime.MinValue, TimeSpan.Zero);

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
