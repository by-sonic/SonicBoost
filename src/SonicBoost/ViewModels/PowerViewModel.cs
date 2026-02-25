using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Power;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class PowerViewModel : ObservableObject
{
    private readonly PowerPlanService _power;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _currentPlan = "Detecting...";
    [ObservableProperty] private string _statusMessage = "";

    public ObservableCollection<PowerPlanInfo> Plans { get; } = [];

    public PowerViewModel(PowerPlanService power)
    {
        _power = power;
    }

    [RelayCommand]
    private async Task LoadPlansAsync()
    {
        IsLoading = true;
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
        await Task.Run(() => _power.EnableUltimatePerformance());
        CurrentPlan = "Ultimate Performance";
        StatusMessage = "Ultimate Performance plan activated";
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SetHighPerformanceAsync()
    {
        await Task.Run(() => _power.SetHighPerformance());
        CurrentPlan = "High Performance";
        StatusMessage = "High Performance plan activated";
    }

    [RelayCommand]
    private async Task DisableHibernationAsync()
    {
        await Task.Run(() => _power.DisableHibernation());
        StatusMessage = "Hibernation disabled";
    }

    [RelayCommand]
    private async Task SetPlanAsync(string guid)
    {
        await Task.Run(() => _power.SetPlan(guid));
        await LoadPlansAsync();
    }
}
