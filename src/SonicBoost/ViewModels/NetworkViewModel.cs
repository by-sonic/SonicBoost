using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Network;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class NetworkViewModel : ObservableObject
{
    private readonly NetworkOptimizer _net;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _selectedDns = "Cloudflare";
    [ObservableProperty] private string _statusMessage = "";

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public List<DnsPreset> DnsPresets { get; } =
    [
        new("Cloudflare", "1.1.1.1", "1.0.0.1"),
        new("Google", "8.8.8.8", "8.8.4.4"),
        new("Quad9", "9.9.9.9", "149.112.112.112"),
        new("OpenDNS", "208.67.222.222", "208.67.220.220"),
    ];

    public NetworkViewModel(NetworkOptimizer net)
    {
        _net = net;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
        Tweaks.Clear();

        await Task.Run(() =>
        {
            var tweaks = _net.GetNetworkTweaks();
            foreach (var t in tweaks) t.IsEnabled = _net.IsTweakApplied(t);
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
            if (tweak.IsEnabled) _net.RevertTweak(tweak);
            else _net.ApplyTweak(tweak);
            tweak.IsEnabled = !tweak.IsEnabled;
        });
        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task ApplyDnsAsync()
    {
        var preset = DnsPresets.FirstOrDefault(p => p.Name == SelectedDns);
        if (preset == null) return;

        await Task.Run(() => _net.SetDns(preset.Primary, preset.Secondary));
        StatusMessage = $"DNS set to {preset.Name} ({preset.Primary})";
    }
}

public record DnsPreset(string Name, string Primary, string Secondary);
