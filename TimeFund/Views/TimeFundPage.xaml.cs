using TimeFund.ViewModels;

namespace TimeFund.Views;

public partial class TimeFundPage : ContentPage
{
	public TimeFundPage(TimeFundViewModel timeFundViewModel)
	{
		InitializeComponent();
		BindingContext = timeFundViewModel;
	}
}