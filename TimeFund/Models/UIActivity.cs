using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeFund.Models;

public partial class UIActivity : ObservableObject
{
    public static readonly UIActivity ZERO_UIACTIVITY = new(Activity.ZERO_ACTIVITY);

    public readonly Activity Activity;

    public int Id
    {
        get { return Activity.Id; }
        set
        {
            if (Activity.Id != value)
            {
                OnPropertyChanged(nameof(Id));
            }
            Activity.Id = value;
        }
    }

    public string Icon
    {
        get { return Activity.Icon; }
        set
        {
            if (Activity.Icon != value)
            {
                OnPropertyChanged(nameof(Icon));
            }
            Activity.Icon = value;
        }
    }

    public string Title
    {
        get { return Activity.Title; }
        set
        {
            if (Activity.Title != value)
            {
                OnPropertyChanged(nameof(Title));
            }
            Activity.Title = value;
        }
    }

    public string Description
    {
        get { return Activity.Description; }
        set
        {
            if (Activity.Description != value)
            {
                OnPropertyChanged(nameof(Description));
            }
            Activity.Description = value; }
    }

    public double Multiplier
    {
        get { return Activity.Multiplier; }
        set
        {
            if (Activity.Multiplier != value)
            {
                OnPropertyChanged(nameof(Multiplier));
            }
            Activity.Multiplier = value; }
    }

    [ObservableProperty]
    private TimeSpan usage;

    public UIActivity(Activity activity, TimeSpan? startingUsage = null)
    {
        this.Activity = activity;
        usage = startingUsage ?? TimeSpan.Zero;
    }

    public UIActivity(int id, string icon, string title, string description, double multiplier, TimeSpan? startingUsage = null)
    {
        Activity = new(id, icon, title, description, multiplier);
        usage = startingUsage ?? TimeSpan.Zero;
    }
}
