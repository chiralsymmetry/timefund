using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeFund.Models;

public partial class UIActivity : ObservableModel
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
                Activity.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    public string Icon
    {
        get { return Activity.Icon; }
        set
        {
            if (Activity.Icon != value)
            {
                Activity.Icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
    }

    public string Title
    {
        get { return Activity.Title; }
        set
        {
            if (Activity.Title != value)
            {
                Activity.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public string Description
    {
        get { return Activity.Description; }
        set
        {
            if (Activity.Description != value)
            {
                Activity.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public double Multiplier
    {
        get { return Activity.Multiplier; }
        set
        {
            if (Activity.Multiplier != value)
            {
                Activity.Multiplier = value;
                OnPropertyChanged(nameof(Multiplier));
            }
        }
    }

    private TimeSpan usage;
    public TimeSpan Usage
    {
        get { return usage; }
        set
        {
            if (usage != value)
            {
                usage = value;
                OnPropertyChanged(nameof(Usage));
            }
        }
    }

    public UIActivity(Activity activity, TimeSpan? startingUsage = null)
    {
        Activity = activity;
        usage = startingUsage ?? TimeSpan.Zero;
    }

    public UIActivity(int id, string icon, string title, string description, double multiplier, TimeSpan? startingUsage = null)
    {
        Activity = new(id, icon, title, description, multiplier);
        usage = startingUsage ?? TimeSpan.Zero;
    }
}
