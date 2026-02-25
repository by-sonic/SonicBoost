using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Logging;
using SonicBoost.Core.Network;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class NetworkViewModel : ObservableObject
{
    private readonly NetworkOptimizer _net;
    private readonly LogService _log;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _selectedDns = "Cloudflare";
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private bool _isError;

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public List<DnsPreset> DnsPresets { get; } =
    [
        new("Cloudflare", "1.1.1.1", "1.0.0.1"),
        new("Google", "8.8.8.8", "8.8.4.4"),
        new("Quad9", "9.9.9.9", "149.112.112.112"),
        new("OpenDNS", "208.67.222.222", "208.67.220.220"),
    ];

    public NetworkViewModel(NetworkOptimizer net, LogService log)
    {
        _net = net;
        _log = log;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
        IsError = false;
        StatusMessage = "Сканирование сетевых настроек...";
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

        var applied = Tweaks.Count(t => t.IsEnabled);
        StatusMessage = $"Сетевых твиков: {Tweaks.Count}, применено: {applied}";
        _log.Info(StatusMessage);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleTweakAsync(TweakItem tweak)
    {
        tweak.IsApplying = true;
        IsError = false;
        var wasEnabled = tweak.IsEnabled;

        try
        {
            await Task.Run(() =>
            {
                if (tweak.IsEnabled)
                    _net.RevertTweak(tweak);
                else
                    _net.ApplyTweak(tweak);
            });

            var verified = _net.IsTweakApplied(tweak);
            tweak.IsEnabled = verified;

            if (wasEnabled && !verified)
            {
                StatusMessage = $"✓ Отменено: {tweak.Name}";
                _log.Info($"Отменено: {tweak.Name}");
            }
            else if (!wasEnabled && verified)
            {
                StatusMessage = $"✓ Применено и проверено: {tweak.Name}";
                _log.Info($"Применено и проверено: {tweak.Name}");
            }
            else
            {
                IsError = true;
                StatusMessage = $"⚠ {tweak.Name} — значение не изменилось после записи";
                _log.Warn($"{tweak.Name} — значение не изменилось. Путь: {tweak.RegistryPath}\\{tweak.RegistryKey}");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            IsError = true;
            StatusMessage = $"⛔ Нет прав: {tweak.Name} — запустите от имени администратора";
            _log.Error($"Нет прав: {tweak.Name}", ex);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка: {tweak.Name} — {ex.Message}";
            _log.Error($"Ошибка при изменении: {tweak.Name}", ex);
        }

        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task ApplyDnsAsync()
    {
        IsError = false;
        var preset = DnsPresets.FirstOrDefault(p => p.Name == SelectedDns);
        if (preset == null) return;

        var (success, output) = await Task.Run(() => _net.SetDns(preset.Primary, preset.Secondary));
        if (success)
        {
            StatusMessage = $"✓ DNS установлен: {preset.Name} ({preset.Primary})";
            _log.Info($"DNS установлен: {preset.Name} — {output}");
        }
        else
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка DNS: {output}";
            _log.Error($"Ошибка DNS: {output}");
        }
    }
}

public record DnsPreset(string Name, string Primary, string Secondary);
