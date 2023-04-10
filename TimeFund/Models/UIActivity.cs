using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeFund.Models;

public partial class UIActivity : ObservableObject
{
    public static readonly UIActivity ZERO_UIACTIVITY = new(Activity.ZERO_ACTIVITY);

    private readonly Activity activity;

    public int Id
    {
        get { return activity.Id; }
        set
        {
            if (activity.Id != value)
            {
                OnPropertyChanged(nameof(Id));
            }
            activity.Id = value;
        }
    }

    public string Icon
    {
        get { return activity.Icon; }
        set
        {
            if (activity.Icon != value)
            {
                OnPropertyChanged(nameof(Icon));
            }
            activity.Icon = value;
        }
    }

    public string Title
    {
        get { return activity.Title; }
        set
        {
            if (activity.Title != value)
            {
                OnPropertyChanged(nameof(Title));
            }
            activity.Title = value;
        }
    }

    public string Description
    {
        get { return activity.Description; }
        set
        {
            if (activity.Description != value)
            {
                OnPropertyChanged(nameof(Description));
            }
            activity.Description = value; }
    }

    public double Multiplier
    {
        get { return activity.Multiplier; }
        set
        {
            if (activity.Multiplier != value)
            {
                OnPropertyChanged(nameof(Multiplier));
            }
            activity.Multiplier = value; }
    }

    [ObservableProperty]
    private TimeSpan usage;

    public UIActivity(Activity activity, TimeSpan? startingUsage = null)
    {
        this.activity = activity;
        usage = startingUsage ?? TimeSpan.Zero;
    }

    public UIActivity(int id, string icon, string title, string description, double multiplier, TimeSpan? startingUsage = null)
    {
        activity = new(id, icon, title, description, multiplier);
        usage = startingUsage ?? TimeSpan.Zero;
    }
}
