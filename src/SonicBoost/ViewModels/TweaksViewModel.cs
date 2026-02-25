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

    public ObservableCollection<TweakItem> Tweaks { get; } = [];

    public TweaksViewModel(TweakEngine engine)
    {
        _engine = engine;
    }

    [RelayCommand]
    private async Task LoadTweaksAsync()
    {
        IsLoading = true;
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

        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleTweakAsync(TweakItem tweak)
    {
        tweak.IsApplying = true;
        await Task.Run(() =>
        {
            if (tweak.IsEnabled)
                _engine.RevertTweak(tweak);
            else
                _engine.ApplyTweak(tweak);

            tweak.IsEnabled = !tweak.IsEnabled;
        });
        tweak.IsApplying = false;
    }

    [RelayCommand]
    private async Task ApplyAllAsync()
    {
        IsLoading = true;
        await Task.Run(() =>
        {
            foreach (var tweak in Tweaks.Where(t => !t.IsEnabled))
            {
                _engine.ApplyTweak(tweak);
                tweak.IsEnabled = true;
            }
        });
        IsLoading = false;
    }
}
