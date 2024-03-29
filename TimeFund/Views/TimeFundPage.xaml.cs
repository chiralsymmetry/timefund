using Microsoft.Extensions.Logging;
using TimeFund.ViewModels;

namespace TimeFund.Views;

public partial class TimeFundPage : ContentPage
{
    private readonly TimeFundViewModel timeFundViewModel;

    public TimeFundPage(TimeFundViewModel timeFundViewModel)
    {
        InitializeComponent();
		BindingContext = timeFundViewModel;
        this.timeFundViewModel = timeFundViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await timeFundViewModel.LoadActivities();
    }
}