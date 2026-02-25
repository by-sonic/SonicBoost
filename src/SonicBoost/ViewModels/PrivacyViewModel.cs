using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Privacy;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class PrivacyViewModel : ObservableObject
{
    private readonly PrivacyService _privacy;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _hostsStatus = "Не применено";

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public PrivacyViewModel(PrivacyService privacy)
    {
        _privacy = privacy;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
        HostsStatus = "Сканирование настроек...";
        Tweaks.Clear();

        await Task.Run(() =>
        {
            var tweaks = _privacy.GetPrivacyTweaks();
            foreach (var t in tweaks) t.IsEnabled = _privacy.IsTweakApplied(t);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in tweaks) Tweaks.Add(t);
            });
        });

        var applied = Tweaks.Count(t => t.IsEnabled);
        HostsStatus = $"Твиков конфиденциальности: {Tweaks.Count}, применено: {applied}";
        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleTweakAsync(TweakItem tweak)
    {
        tweak.IsApplying = true;
        try
        {
            await Task.Run(() =>
            {
                if (tweak.IsEnabled)
                    _privacy.RevertTweak(tweak);
                else
                    _privacy.ApplyTweak(tweak);
                tweak.IsEnabled = !tweak.IsEnabled;
            });
            HostsStatus = tweak.IsEnabled
                ? $"Применено: {tweak.Name}"
                : $"Отменено: {tweak.Name}";
        }
        catch (Exception ex)
        {
            HostsStatus = $"Ошибка: {tweak.Name} — {ex.Message}";
        }
        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task BlockTelemetryHostsAsync()
    {
        await Task.Run(() => _privacy.BlockTelemetryHosts());
        HostsStatus = "Хосты телеметрии заблокированы";
    }

    [RelayCommand]
    private async Task ApplyAllAsync()
    {
        IsLoading = true;
        int count = 0;
        await Task.Run(() =>
        {
            foreach (var t in Tweaks.Where(t => !t.IsEnabled))
            {
                _privacy.ApplyTweak(t);
                t.IsEnabled = true;
                count++;
            }
            _privacy.BlockTelemetryHosts();
        });
        HostsStatus = count > 0
            ? $"Применено: {count} настроек. Хосты телеметрии заблокированы"
            : "Все настройки уже применены. Хосты заблокированы";
        IsLoading = false;
    }
}
