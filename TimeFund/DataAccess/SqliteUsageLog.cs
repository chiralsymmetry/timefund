using SQLite;

namespace TimeFund.DataAccess;

internal class SqliteUsageLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public double Multiplier { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
}
