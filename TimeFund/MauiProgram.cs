using Microsoft.Extensions.Logging;
using TimeFund.DataAccess;
using TimeFund.Models;
using TimeFund.ViewModels;
using TimeFund.Views;

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
		var running = new Activity(0, "🏃", "Running", "Running is a great way to get in shape.", 1.0);
		var lifting = new Activity(0, "🏋️", "Weightlifting", "Weightlifting is a great way to get in shape.", 2.0);
		var swimming = new Activity(0, "🏊", "Swimming", "Swimming is a great way to get in shape.", 1.5);
		var videogames = new Activity(0, "🎮", "Video Games", "Video games are a great way to relax.", -1.0);
		var tv = new Activity(0, "📺", "TV", "TV is a great way to relax.", -1.5);
		var phone = new Activity(0, "📱", "Phone", "Phone is a great way to relax.", -2.0);

        var dataAccess = new MemoryDataAccess();
        dataAccess.InsertActivityAsync(running).Wait();
        dataAccess.InsertActivityAsync(lifting).Wait();
        dataAccess.InsertActivityAsync(swimming).Wait();
        dataAccess.InsertActivityAsync(videogames).Wait();
        dataAccess.InsertActivityAsync(tv).Wait();
        dataAccess.InsertActivityAsync(phone).Wait();

        var time = DateTime.UtcNow.AddDays(-3);
        // Day 1
        dataAccess.InsertUsageLogAsync(running, time, TimeSpan.FromHours(0.75)).Wait();
        time += TimeSpan.FromHours(0.75 + 1.27);
        dataAccess.InsertUsageLogAsync(lifting, time, TimeSpan.FromHours(1.23)).Wait();
        time += TimeSpan.FromHours(1.23 + 2.58);
        dataAccess.InsertUsageLogAsync(swimming, time, TimeSpan.FromHours(1.51)).Wait();
        time += TimeSpan.FromHours(1.51 + 2.12);
        dataAccess.InsertUsageLogAsync(videogames, time, TimeSpan.FromHours(2.35)).Wait();
        time += TimeSpan.FromHours(2.35 + 1.46);
        dataAccess.InsertUsageLogAsync(tv, time, TimeSpan.FromHours(1.73)).Wait();
        time += TimeSpan.FromHours(1.73 + 9.64);
        // Day 2
        dataAccess.InsertUsageLogAsync(running, time, TimeSpan.FromHours(0.85)).Wait();
        time += TimeSpan.FromHours(0.85 + 1.75);
        dataAccess.InsertUsageLogAsync(lifting, time, TimeSpan.FromHours(1.30)).Wait();
        time += TimeSpan.FromHours(1.30 + 2.58);
        dataAccess.InsertUsageLogAsync(swimming, time, TimeSpan.FromHours(1.58)).Wait();
        time += TimeSpan.FromHours(1.58 + 2.09);
        dataAccess.InsertUsageLogAsync(phone, time, TimeSpan.FromHours(2.41)).Wait();
        time += TimeSpan.FromHours(2.41 + 1.53);
        dataAccess.InsertUsageLogAsync(tv, time, TimeSpan.FromHours(1.75)).Wait();
        time += TimeSpan.FromHours(1.75 + 11.41);
        // Day 3
        dataAccess.InsertUsageLogAsync(running, time, TimeSpan.FromHours(0.65)).Wait();
        time += TimeSpan.FromHours(0.65 + 2.25);
        dataAccess.InsertUsageLogAsync(lifting, time, TimeSpan.FromHours(1.13)).Wait();
        time += TimeSpan.FromHours(1.13 + 2.51);
        dataAccess.InsertUsageLogAsync(swimming, time, TimeSpan.FromHours(1.59)).Wait();
        time += TimeSpan.FromHours(1.59 + 1.75);
        dataAccess.InsertUsageLogAsync(videogames, time, TimeSpan.FromHours(1.87)).Wait();
        time += TimeSpan.FromHours(1.87 + 2.25);
        dataAccess.InsertUsageLogAsync(phone, time, TimeSpan.FromHours(2.15)).Wait();
        time += TimeSpan.FromHours(2.15 + 1.75);
        dataAccess.InsertUsageLogAsync(tv, time, TimeSpan.FromHours(1.40)).Wait();
        time += TimeSpan.FromHours(1.40);

        builder.Services.AddSingleton(typeof(IDataAccess), dataAccess);
		builder.Services.AddSingleton<TimeFundViewModel>();
		builder.Services.AddSingleton<TimeFundPage>();
        builder.Services.AddSingleton<AllActivitiesViewModel>();
        builder.Services.AddSingleton<AllActivitiesPage>();
        builder.Services.AddSingleton<AllUsageLogsViewModel>();
        builder.Services.AddSingleton<AllUsageLogsPage>();
        builder.Services.AddSingleton<SingleActivityViewModel>();
        builder.Services.AddSingleton<SingleActivityPage>();

        return builder.Build();
	}
}
