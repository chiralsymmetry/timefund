using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimeFund.DataAccess;
using TimeFund.Models;
using TimeFund.Views;

namespace TimeFund.ViewModels;

public class AllActivitiesViewModel : ObservableViewModel
{
    private readonly IDataAccess dataAccess;

    public ObservableCollection<UIActivity> AllActivities { get; set; }
    public UIActivity SelectedActivity { get; set; } = UIActivity.ZERO_UIACTIVITY;

    public AllActivitiesViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
        var storedActivities = Task.Run(dataAccess.GetAllActivitiesAsync).Result;
        AllActivities = new();
        Task.Run(LoadActivities);
    }

    public async Task LoadActivities()
    {
        var idToExistingActivities = AllActivities.ToDictionary(a => a.Id);
        var freshActivities = (await dataAccess.GetAllActivitiesAsync()).ToList();
        AllActivities.Clear();
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var today = DateTime.UtcNow;
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
                AllActivities.Add(existingActivity);
            }
            else
            {
                AllActivities.Add(new UIActivity(freshActivity, totalUsage));
            }
        }
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
    private RelayCommand? activitySelectedCommand;
    public IRelayCommand ActivitySelectedCommand => activitySelectedCommand ??= new RelayCommand(ActivitySelected);

    private async void AddActivity()
    {
        SelectedActivity = UIActivity.ZERO_UIACTIVITY;
        OnPropertyChanged(nameof(SelectedActivity));
        await Shell.Current.GoToAsync(nameof(SingleActivityPage), true, new Dictionary<string, object> { { nameof(SingleActivityViewModel.ExaminedActivity), new Activity(title: "New Activity") } });
    }
    private RelayCommand? addActivityCommand;
    public IRelayCommand AddActivityCommand => addActivityCommand ??= new RelayCommand(AddActivity);
}
