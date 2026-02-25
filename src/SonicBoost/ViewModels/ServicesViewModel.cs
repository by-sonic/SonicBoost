using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Logging;
using SonicBoost.Core.Services;
using SonicBoost.Core.Tweaks.Models;
using System.Collections.ObjectModel;
using System.ServiceProcess;

namespace SonicBoost.ViewModels;

public partial class ServicesViewModel : ObservableObject
{
    private readonly ServiceManager _manager;
    private readonly LogService _log;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private bool _isError;

    public ObservableCollection<ServiceItem> Services { get; } = [];

    public ServicesViewModel(ServiceManager manager, LogService log)
    {
        _manager = manager;
        _log = log;
    }

    [RelayCommand]
    private async Task LoadServicesAsync()
    {
        IsLoading = true;
        IsError = false;
        StatusMessage = "Сканирование служб...";
        Services.Clear();

        var items = await Task.Run(() => _manager.GetOptimizableServices());
        foreach (var item in items) Services.Add(item);

        var disabled = items.Count(s => s.IsDisabledByUser);
        StatusMessage = $"Найдено служб: {items.Count}, отключено: {disabled}";
        _log.Info(StatusMessage);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleServiceAsync(ServiceItem item)
    {
        IsError = false;
        try
        {
            await Task.Run(() =>
            {
                if (item.IsDisabledByUser)
                    _manager.EnableService(item);
                else
                    _manager.DisableService(item);
            });

            await Task.Run(() =>
            {
                using var svc = new ServiceController(item.ServiceName);
                svc.Refresh();
                item.IsDisabledByUser = svc.StartType == ServiceStartMode.Disabled;
                item.Status = svc.Status.ToString();
                item.IsRunning = svc.Status == ServiceControllerStatus.Running;
            });

            StatusMessage = item.IsDisabledByUser
                ? $"✓ Отключена и проверена: {item.DisplayName}"
                : $"✓ Включена: {item.DisplayName}";
            _log.Info(StatusMessage);
        }
        catch (UnauthorizedAccessException ex)
        {
            IsError = true;
            StatusMessage = $"⛔ Нет прав: {item.DisplayName} — запустите от имени администратора";
            _log.Error($"Нет прав: {item.DisplayName}", ex);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка: {item.DisplayName} — {ex.Message}";
            _log.Error($"Ошибка службы: {item.DisplayName}", ex);
        }
    }

    [RelayCommand]
    private async Task DisableAllSafeAsync()
    {
        IsLoading = true;
        IsError = false;
        int count = 0;
        int failed = 0;

        await Task.Run(() =>
        {
            foreach (var svc in Services.Where(s => s.Risk == ServiceRisk.Safe && !s.IsDisabledByUser).ToList())
            {
                try
                {
                    _manager.DisableService(svc);
                    svc.IsDisabledByUser = true;
                    count++;
                    _log.Info($"Отключена: {svc.DisplayName}");
                }
                catch (Exception ex)
                {
                    failed++;
                    _log.Error($"DisableAllSafe — {svc.DisplayName}", ex);
                }
            }
        });

        if (failed > 0)
        {
            IsError = true;
            StatusMessage = $"Отключено: {count}, ошибок: {failed}";
        }
        else
        {
            StatusMessage = count > 0
                ? $"✓ Отключено безопасных служб: {count}. Готово!"
                : "Все безопасные службы уже отключены";
        }
        _log.Info(StatusMessage);
        IsLoading = false;
    }
}
