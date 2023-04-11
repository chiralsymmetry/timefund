using TimeFund.Models;
using TimeFund.ViewModels;

namespace TimeFund.Views;

public partial class SingleActivityPage : ContentPage
{
    private readonly SingleActivityViewModel singleActivityViewModel;

    public SingleActivityPage(SingleActivityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        singleActivityViewModel = viewModel;
    }

    private void MultiplierEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        singleActivityViewModel?.OnPropertyChanged(nameof(SingleActivityViewModel.MultiplierString));
    }
}