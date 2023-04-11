using TimeFund.Views;

namespace TimeFund;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SingleActivityPage), typeof(SingleActivityPage));
    }
}
