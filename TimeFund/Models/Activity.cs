namespace TimeFund.Models;

public class Activity
{
    public int Id { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Multiplier { get; set; } = 0;
}
