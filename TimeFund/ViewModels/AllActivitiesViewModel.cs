using CommunityToolkit.Mvvm.Input;
using TimeFund.DataAccess;
using TimeFund.Models;
using TimeFund.Views;

namespace TimeFund.ViewModels;

public class AllActivitiesViewModel : ObservableViewModel
{
    private readonly IDataAccess dataAccess;

    private List<UIActivity> allActivities = new();
    public List<UIActivity> AllActivities
    {
        get => allActivities;
        set
        {
            if (allActivities != value)
            {
                allActivities = value;
                OnPropertyChanged(nameof(AllActivities));
            }
        }
    }
    public UIActivity SelectedActivity { get; set; } = UIActivity.ZERO_UIACTIVITY;

    public AllActivitiesViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
        var storedActivities = Task.Run(dataAccess.GetAllActivitiesAsync).Result;
        //Task.Run(LoadActivities);
    }

    public async Task LoadActivities()
    {
        var idToExistingActivities = AllActivities.ToDictionary(a => a.Id);
        var freshActivities = (await dataAccess.GetAllActivitiesAsync()).ToList();
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var today = DateTime.UtcNow;
        var activitiesToAdd = new List<UIActivity>();
        foreach (var freshActivity in freshActivities)
        {
            var totalUsage = await dataAccess.GetTotalUsageOverlappingIntervalForActivityAsync(freshActivity, yesterday, today);
            if (idToExistingActivities.TryGetValue(freshActivity.Id, out var existingActivity))
            {
                existingActivity.Icon = freshActivity.Icon;
                existingActivity.Title = freshActivity.Title;
                existingActivity.Description = freshActivity.Description;
                existingActivity.Multiplier = freshActivity.Multiplier;
                existingActivity.Usage = totalUsage;
                activitiesToAdd.Add(existingActivity);
            }
            else
            {
                activitiesToAdd.Add(new UIActivity(freshActivity, totalUsage));
            }
        }
        allActivities = new(activitiesToAdd);
        OnPropertyChanged(nameof(AllActivities));
        SelectedActivity = UIActivity.ZERO_UIACTIVITY;
    }

    private async void ActivitySelected()
    {
        if (SelectedActivity.Id > 0)
        {
            var activity = SelectedActivity.Activity;
            SelectedActivity = UIActivity.ZERO_UIACTIVITY;
            OnPropertyChanged(nameof(SelectedActivity));
            await Shell.Current.GoToAsync(nameof(SingleActivityPage), true, new Dictionary<string, object> { { nameof(SingleActivityViewModel.ExaminedActivity), activity } });
        }
    }
    public IRelayCommand ActivitySelectedCommand => new RelayCommand(ActivitySelected);

    private async void AddActivity()
    {
        SelectedActivity = UIActivity.ZERO_UIACTIVITY;
        OnPropertyChanged(nameof(SelectedActivity));
        await Shell.Current.GoToAsync(nameof(SingleActivityPage), true, new Dictionary<string, object> { { nameof(SingleActivityViewModel.ExaminedActivity), new Activity(title: "New Activity") } });
    }
    public IRelayCommand AddActivityCommand => new RelayCommand(AddActivity);
}
