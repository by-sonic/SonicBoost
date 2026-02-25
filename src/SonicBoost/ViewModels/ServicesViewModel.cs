using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Services;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class ServicesViewModel : ObservableObject
{
    private readonly ServiceManager _manager;

    [ObservableProperty] private bool _isLoading;

    public ObservableCollection<ServiceItem> Services { get; } = [];

    public ServicesViewModel(ServiceManager manager)
    {
        _manager = manager;
    }

    [RelayCommand]
    private async Task LoadServicesAsync()
    {
        IsLoading = true;
        Services.Clear();

        var items = await Task.Run(() => _manager.GetOptimizableServices());
        foreach (var item in items) Services.Add(item);

        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleServiceAsync(ServiceItem item)
    {
        await Task.Run(() =>
        {
            if (item.IsDisabledByUser)
                _manager.EnableService(item);
            else
                _manager.DisableService(item);

            item.IsDisabledByUser = !item.IsDisabledByUser;
        });
    }

    [RelayCommand]
    private async Task DisableAllSafeAsync()
    {
        IsLoading = true;
        await Task.Run(() =>
        {
            foreach (var svc in Services.Where(s => s.Risk == ServiceRisk.Safe && !s.IsDisabledByUser))
            {
                _manager.DisableService(svc);
                svc.IsDisabledByUser = true;
            }
        });
        IsLoading = false;
    }
}
