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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReplacePicker();
        await allUsageLogsViewModel.LoadUsageLogs();
    }

    private async Task ReplacePicker()
    {
        // It seems that Pickers' items sources cannot be updated. Workaround: create a new one.
        await allUsageLogsViewModel.LoadActivities();
        var oldPicker = ActivityPicker;
        var newPicker = new Picker();
        newPicker.SetBinding(Picker.ItemsSourceProperty, new Binding("Activities") { Mode = BindingMode.OneTime });
        newPicker.SetBinding(Picker.SelectedItemProperty, new Binding("SelectedActivity") { Mode = BindingMode.TwoWay });
        newPicker.ItemDisplayBinding = new Binding("Title");
        newPicker.BindingContext = oldPicker.BindingContext;
        newPicker.SelectedItem = allUsageLogsViewModel.SelectedActivity;

        var parent = (Layout)ActivityPicker.Parent;
        var index = parent.Children.IndexOf(ActivityPicker);
        parent.Children.RemoveAt(index);
        parent.Children.Insert(index, newPicker);
        ActivityPicker = newPicker;
    }
}