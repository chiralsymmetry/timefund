using CommunityToolkit.Mvvm.Input;
using Humanizer;
using TimeFund.DataAccess;
using TimeFund.Models;

namespace TimeFund.ViewModels;

[QueryProperty(nameof(ExaminedUsageLog), nameof(ExaminedUsageLog))]
public class SingleUsageLogViewModel : ObservableViewModel
{
    private readonly IDataAccess dataAccess;

    private UsageLog examinedUsageLog = UsageLog.ZERO_USAGELOG;
    public UsageLog ExaminedUsageLog
    {
        get => examinedUsageLog;
        set
        {
            if (examinedUsageLog != value)
            {
                examinedUsageLog = value;
                OnPropertyChanged(nameof(ExaminedUsageLog));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Description));
            }
        }
    }
    public string Title => $"{ExaminedUsageLog.Activity.Title}";
    private TimeSpan MultipliedTime => ExaminedUsageLog.Duration * Math.Abs(ExaminedUsageLog.Multiplier);
    private string DescriptionStart => $"{ExaminedUsageLog.StartTime.ToLocalTime().ToString().Transform(To.SentenceCase)} the timer for {ExaminedUsageLog.Activity.Title} was activated.";
    private string DescriptionEnd => $"After {ExaminedUsageLog.Duration.Humanize(precision: 3, minUnit: Humanizer.Localisation.TimeUnit.Second)}, on {ExaminedUsageLog.EndTime.ToLocalTime()}, the timer was stopped.";
    private string DescriptionDuration => $"The multiplier was {ExaminedUsageLog.Multiplier:F2}, which means that {MultipliedTime.Humanize(precision: 3, minUnit: Humanizer.Localisation.TimeUnit.Second)} was {((ExaminedUsageLog.Activity.Multiplier < 0) ? "consumed from" : "added to")} the TimeFund.";
    public string Description => $"{DescriptionStart}\n\n{DescriptionEnd}\n\n{DescriptionDuration}";

    public SingleUsageLogViewModel(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    private async void DeleteUsageLog()
    {
        if (ExaminedUsageLog != UsageLog.ZERO_USAGELOG)
        {
            var response = true;
            if (ExaminedUsageLog.Id > 0)
            {
                response = await Shell.Current.DisplayAlert(
                    "Delete Usage Log",
                    $"Do you really want to delete this usage log for \"{ExaminedUsageLog.Activity.Title}\"? It has {ExaminedUsageLog.Duration.Humanize()} of log time.",
                    "Delete", "Cancel");
            }
            if (response)
            {
                var usageLog = ExaminedUsageLog;
                ExaminedUsageLog = UsageLog.ZERO_USAGELOG;
                await dataAccess.DeleteUsageLogAsync(usageLog);
                await Shell.Current.Navigation.PopAsync();
            }
        }
    }
    public IRelayCommand DeleteUsageLogCommand => new RelayCommand(DeleteUsageLog);
}
