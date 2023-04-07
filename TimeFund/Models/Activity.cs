namespace TimeFund.Models;

public class Activity
{
    public int Id { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Multiplier { get; set; }

    public Activity(int id = 0, string icon = "", string title = "", string description = "", double multiplier = 0)
    {
        Id = id;
        Icon = icon;
        Title = title;
        Description = description;
        Multiplier = multiplier;
    }
}
