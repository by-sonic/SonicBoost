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
    [ObservableProperty] private string _statusMessage = "";

    public ObservableCollection<ServiceItem> Services { get; } = [];

    public ServicesViewModel(ServiceManager manager)
    {
        _manager = manager;
    }

    [RelayCommand]
    private async Task LoadServicesAsync()
    {
        IsLoading = true;
        StatusMessage = "Сканирование служб...";
        Services.Clear();

        var items = await Task.Run(() => _manager.GetOptimizableServices());
        foreach (var item in items) Services.Add(item);

        var disabled = items.Count(s => s.IsDisabledByUser);
        StatusMessage = $"Найдено служб: {items.Count}, отключено: {disabled}";
        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleServiceAsync(ServiceItem item)
    {
        var wasDisabled = item.IsDisabledByUser;
        try
        {
            await Task.Run(() =>
            {
                if (item.IsDisabledByUser)
                    _manager.EnableService(item);
                else
                    _manager.DisableService(item);

                item.IsDisabledByUser = !item.IsDisabledByUser;
            });
            StatusMessage = item.IsDisabledByUser
                ? $"Отключена: {item.DisplayName}"
                : $"Включена: {item.DisplayName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {item.DisplayName} — {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DisableAllSafeAsync()
    {
        IsLoading = true;
        int count = 0;
        await Task.Run(() =>
        {
            foreach (var svc in Services.Where(s => s.Risk == ServiceRisk.Safe && !s.IsDisabledByUser))
            {
                _manager.DisableService(svc);
                svc.IsDisabledByUser = true;
                count++;
            }
        });
        StatusMessage = count > 0
            ? $"Отключено безопасных служб: {count}. Готово!"
            : "Все безопасные службы уже отключены";
        IsLoading = false;
    }
}
