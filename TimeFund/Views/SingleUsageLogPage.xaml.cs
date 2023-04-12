using TimeFund.ViewModels;

namespace TimeFund.Views;

public partial class SingleUsageLogPage : ContentPage
{
	public SingleUsageLogPage(SingleUsageLogViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}