using TimeFund.ViewModels;

namespace TimeFund.Views;

public partial class AllUsageLogsPage : ContentPage
{
    private readonly AllUsageLogsViewModel allUsageLogsViewModel;

    public AllUsageLogsPage(AllUsageLogsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        allUsageLogsViewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(allUsageLogsViewModel.LoadUsageLogs);
    }
}