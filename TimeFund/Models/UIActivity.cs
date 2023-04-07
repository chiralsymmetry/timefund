using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeFund.Models;

public partial class UIActivity : ObservableObject
{
    public static readonly UIActivity ZERO_ACTIVITY = new(new());

    private readonly Activity activity;

    public int Id => activity.Id;
    public string Icon => activity.Icon;
    public string Title => activity.Title;
    public string Description => activity.Description;
    public double Multiplier => activity.Multiplier;

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
