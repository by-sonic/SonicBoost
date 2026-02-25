using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Tweaks;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class TweaksViewModel : ObservableObject
{
    private readonly TweakEngine _engine;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "";

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public TweaksViewModel(TweakEngine engine)
    {
        _engine = engine;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
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
        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleTweakAsync(TweakItem tweak)
    {
        tweak.IsApplying = true;
        var wasEnabled = tweak.IsEnabled;
        try
        {
            await Task.Run(() =>
            {
                if (tweak.IsEnabled)
                    _engine.RevertTweak(tweak);
                else
                    _engine.ApplyTweak(tweak);

                tweak.IsEnabled = !tweak.IsEnabled;
            });
            StatusMessage = tweak.IsEnabled
                ? $"Применено: {tweak.Name}"
                : $"Отменено: {tweak.Name}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {tweak.Name} — {ex.Message}";
        }
        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task ApplyAllAsync()
    {
        IsLoading = true;
        var toApply = Tweaks.Where(t => !t.IsEnabled).ToList();
        int count = 0;
        await Task.Run(() =>
        {
            foreach (var tweak in toApply)
            {
                _engine.ApplyTweak(tweak);
                tweak.IsEnabled = true;
                count++;
            }
        });
        StatusMessage = count > 0
            ? $"Применено твиков: {count}. Готово!"
            : "Все твики уже применены";
        IsLoading = false;
    }
}
