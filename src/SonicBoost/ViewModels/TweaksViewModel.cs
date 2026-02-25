using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Logging;
using SonicBoost.Core.Tweaks;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class TweaksViewModel : ObservableObject
{
    private readonly TweakEngine _engine;
    private readonly LogService _log;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private bool _isError;

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public TweaksViewModel(TweakEngine engine, LogService log)
    {
        _engine = engine;
        _log = log;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
        IsError = false;
        StatusMessage = "Сканирование текущих настроек...";
        Tweaks.Clear();

        await Task.Run(() =>
        {
            var tweaks = _engine.GetAllTweaks();
            foreach (var tweak in tweaks)
            {
                tweak.IsEnabled = _engine.IsTweakApplied(tweak);
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in tweaks) Tweaks.Add(t);
            });
        });

        var applied = Tweaks.Count(t => t.IsEnabled);
        StatusMessage = $"Загружено {Tweaks.Count} твиков, из них применено: {applied}";
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
                    _engine.RevertTweak(tweak);
                else
                    _engine.ApplyTweak(tweak);
            });

            var verified = _engine.IsTweakApplied(tweak);
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
    private async Task ApplyAllAsync()
    {
        IsLoading = true;
        IsError = false;
        var toApply = Tweaks.Where(t => !t.IsEnabled).ToList();
        int applied = 0;
        int failed = 0;
        var errors = new List<string>();

        await Task.Run(() =>
        {
            foreach (var tweak in toApply)
            {
                try
                {
                    _engine.ApplyTweak(tweak);
                    tweak.IsEnabled = _engine.IsTweakApplied(tweak);
                    if (tweak.IsEnabled) applied++;
                    else { failed++; errors.Add(tweak.Name); }
                }
                catch (Exception ex)
                {
                    failed++;
                    errors.Add($"{tweak.Name}: {ex.Message}");
                    _log.Error($"ApplyAll — {tweak.Name}", ex);
                }
            }
        });

        if (failed > 0)
        {
            IsError = true;
            StatusMessage = $"Применено: {applied}, ошибок: {failed}. {string.Join("; ", errors.Take(3))}";
        }
        else
        {
            StatusMessage = applied > 0
                ? $"✓ Применено твиков: {applied}. Готово!"
                : "Все твики уже применены";
        }
        _log.Info(StatusMessage);
        IsLoading = false;
    }
}
