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
    [ObservableProperty] private string _hostsStatus = "Not applied";

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public PrivacyViewModel(PrivacyService privacy)
    {
        _privacy = privacy;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
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

        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleTweakAsync(TweakItem tweak)
    {
        tweak.IsApplying = true;
        await Task.Run(() =>
        {
            if (tweak.IsEnabled)
                _privacy.RevertTweak(tweak);
            else
                _privacy.ApplyTweak(tweak);
            tweak.IsEnabled = !tweak.IsEnabled;
        });
        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task BlockTelemetryHostsAsync()
    {
        await Task.Run(() => _privacy.BlockTelemetryHosts());
        HostsStatus = "Telemetry hosts blocked";
    }

    [RelayCommand]
    private async Task ApplyAllAsync()
    {
        IsLoading = true;
        await Task.Run(() =>
        {
            foreach (var t in Tweaks.Where(t => !t.IsEnabled))
            {
                _privacy.ApplyTweak(t);
                t.IsEnabled = true;
            }
            _privacy.BlockTelemetryHosts();
        });
        HostsStatus = "Telemetry hosts blocked";
        IsLoading = false;
    }
}
