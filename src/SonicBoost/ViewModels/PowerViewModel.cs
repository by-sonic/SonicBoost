using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Logging;
using SonicBoost.Core.Power;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class PowerViewModel : ObservableObject
{
    private readonly PowerPlanService _power;
    private readonly LogService _log;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _currentPlan = "Определение...";
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private bool _isError;

    public ObservableCollection<PowerPlanInfo> Plans { get; } = [];

    public PowerViewModel(PowerPlanService power, LogService log)
    {
        _power = power;
        _log = log;
    }

    [RelayCommand]
    private async Task LoadPlansAsync()
    {
        IsLoading = true;
        IsError = false;
        Plans.Clear();

        await Task.Run(() =>
        {
            var current = _power.GetCurrentPlan();
            CurrentPlan = current.Name;

            var plans = _power.GetAllPlans();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var p in plans) Plans.Add(p);
            });
        });

        IsLoading = false;
    }

    [RelayCommand]
    private async Task EnableUltimatePerformanceAsync()
    {
        IsLoading = true;
        IsError = false;
        try
        {
            await Task.Run(() => _power.EnableUltimatePerformance());
            CurrentPlan = "Максимальная производительность";
            StatusMessage = "✓ План «Максимальная производительность» активирован";
            _log.Info(StatusMessage);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка: {ex.Message}";
            _log.Error("EnableUltimatePerformance", ex);
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SetHighPerformanceAsync()
    {
        IsError = false;
        try
        {
            await Task.Run(() => _power.SetHighPerformance());
            CurrentPlan = "Высокая производительность";
            StatusMessage = "✓ План «Высокая производительность» активирован";
            _log.Info(StatusMessage);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка: {ex.Message}";
            _log.Error("SetHighPerformance", ex);
        }
    }

    [RelayCommand]
    private async Task DisableHibernationAsync()
    {
        IsError = false;
        try
        {
            await Task.Run(() => _power.DisableHibernation());
            StatusMessage = "✓ Гибернация отключена";
            _log.Info(StatusMessage);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка: {ex.Message}";
            _log.Error("DisableHibernation", ex);
        }
    }

    [RelayCommand]
    private async Task SetPlanAsync(string guid)
    {
        try
        {
            await Task.Run(() => _power.SetPlan(guid));
            await LoadPlansAsync();
            _log.Info($"План электропитания изменён: {guid}");
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка: {ex.Message}";
            _log.Error("SetPlan", ex);
        }
    }
}
