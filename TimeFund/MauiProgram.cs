using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimeFund.DataAccess;
using TimeFund.Models;
using TimeFund.ViewModels;

namespace TimeFund;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var dataAccess = new MemoryDataAccess();
		dataAccess.InsertAsync(new Activity(0, "🏃", "Running", "Running is a great way to get in shape.", 1.0)).Wait();
		dataAccess.InsertAsync(new Activity(0, "🏋️", "Weightlifting", "Weightlifting is a great way to get in shape.", 2.0)).Wait();
		dataAccess.InsertAsync(new Activity(0, "🏊", "Swimming", "Swimming is a great way to get in shape.", 1.5)).Wait();
		dataAccess.InsertAsync(new Activity(0, "🎮", "Video Games", "Video games are a great way to relax.", -1.0)).Wait();
		dataAccess.InsertAsync(new Activity(0, "📺", "TV", "TV is a great way to relax.", -1.5)).Wait();
		dataAccess.InsertAsync(new Activity(0, "📱", "Phone", "Phone is a great way to relax.", -2.0)).Wait();

		builder.Services.AddSingleton(typeof(IDataAccess), dataAccess);
		builder.Services.AddSingleton<TimeFundViewModel>();

		return builder.Build();
	}
}
