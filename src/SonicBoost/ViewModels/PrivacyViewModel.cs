using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Logging;
using SonicBoost.Core.Privacy;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class PrivacyViewModel : ObservableObject
{
    private readonly PrivacyService _privacy;
    private readonly LogService _log;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _hostsStatus = "Не применено";
    [ObservableProperty] private bool _isError;

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public PrivacyViewModel(PrivacyService privacy, LogService log)
    {
        _privacy = privacy;
        _log = log;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
        IsError = false;
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
        _log.Info(HostsStatus);
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
                    _privacy.RevertTweak(tweak);
                else
                    _privacy.ApplyTweak(tweak);
            });

            var verified = _privacy.IsTweakApplied(tweak);
            tweak.IsEnabled = verified;

            if (wasEnabled && !verified)
            {
                HostsStatus = $"✓ Отменено: {tweak.Name}";
                _log.Info($"Отменено: {tweak.Name}");
            }
            else if (!wasEnabled && verified)
            {
                HostsStatus = $"✓ Применено и проверено: {tweak.Name}";
                _log.Info($"Применено и проверено: {tweak.Name}");
            }
            else
            {
                IsError = true;
                HostsStatus = $"⚠ {tweak.Name} — значение не изменилось";
                _log.Warn($"{tweak.Name} — значение не изменилось. Путь: {tweak.RegistryPath}\\{tweak.RegistryKey}");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            IsError = true;
            HostsStatus = $"⛔ Нет прав: {tweak.Name} — запустите от имени администратора";
            _log.Error($"Нет прав: {tweak.Name}", ex);
        }
        catch (Exception ex)
        {
            IsError = true;
            HostsStatus = $"❌ Ошибка: {tweak.Name} — {ex.Message}";
            _log.Error($"Ошибка: {tweak.Name}", ex);
        }

        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task BlockTelemetryHostsAsync()
    {
        IsError = false;
        try
        {
            await Task.Run(() => _privacy.BlockTelemetryHosts());
            HostsStatus = "✓ Хосты телеметрии заблокированы";
            _log.Info("Хосты телеметрии заблокированы");
        }
        catch (Exception ex)
        {
            IsError = true;
            HostsStatus = $"❌ Ошибка: {ex.Message}";
            _log.Error("Ошибка блокировки хостов", ex);
        }
    }

    [RelayCommand]
    private async Task ApplyAllAsync()
    {
        IsLoading = true;
        IsError = false;
        int applied = 0;
        int failed = 0;

        await Task.Run(() =>
        {
            foreach (var t in Tweaks.Where(t => !t.IsEnabled).ToList())
            {
                try
                {
                    _privacy.ApplyTweak(t);
                    t.IsEnabled = _privacy.IsTweakApplied(t);
                    if (t.IsEnabled) applied++;
                    else failed++;
                }
                catch (Exception ex)
                {
                    failed++;
                    _log.Error($"ApplyAll — {t.Name}", ex);
                }
            }

            try { _privacy.BlockTelemetryHosts(); }
            catch (Exception ex) { _log.Error("BlockTelemetryHosts", ex); }
        });

        if (failed > 0)
        {
            IsError = true;
            HostsStatus = $"Применено: {applied}, ошибок: {failed}. Хосты заблокированы";
        }
        else
        {
            HostsStatus = applied > 0
                ? $"✓ Применено: {applied} настроек. Хосты заблокированы"
                : "Все настройки уже применены. Хосты заблокированы";
        }
        _log.Info(HostsStatus);
        IsLoading = false;
    }
}
