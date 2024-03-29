﻿using SQLite;

namespace TimeFund.Models;

public class Activity
{
    public static readonly Activity ZERO_ACTIVITY = new(0, string.Empty, string.Empty, string.Empty, 0);

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Multiplier { get; set; }

    public Activity() : this(0, string.Empty, string.Empty, string.Empty, 0)
    {
    }

    public Activity(int id = 0, string icon = "", string title = "", string description = "", double multiplier = 0)
    {
        Id = id;
        Icon = icon;
        Title = title;
        Description = description;
        Multiplier = multiplier;
    }
}
