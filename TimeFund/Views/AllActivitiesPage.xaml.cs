using TimeFund.ViewModels;

namespace TimeFund.Views;

public partial class AllActivitiesPage : ContentPage
{
    private readonly AllActivitiesViewModel allActivitiesViewModel;

    public AllActivitiesPage(AllActivitiesViewModel allActivitiesViewModel)
	{
		InitializeComponent();
		BindingContext = allActivitiesViewModel;
        this.allActivitiesViewModel = allActivitiesViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(allActivitiesViewModel.LoadActivities);
    }
}