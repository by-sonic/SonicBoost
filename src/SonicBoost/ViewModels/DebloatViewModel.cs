using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Debloat;
using SonicBoost.Core.Logging;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class DebloatViewModel : ObservableObject
{
    private readonly DebloatService _debloat;
    private readonly LogService _log;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private string _tempCleanResult = "";
    [ObservableProperty] private bool _isError;

    public ObservableCollection<BloatApp> Apps { get; } = [];

    public DebloatViewModel(DebloatService debloat, LogService log)
    {
        _debloat = debloat;
        _log = log;
    }

    [RelayCommand]
    private async Task LoadAppsAsync()
    {
        IsLoading = true;
        IsError = false;
        Apps.Clear();
        StatusMessage = "Сканирование установленных приложений...";

        var items = await Task.Run(() => _debloat.GetInstalledBloatware());
        foreach (var item in items) Apps.Add(item);

        StatusMessage = $"Найдено приложений для удаления: {items.Count}";
        _log.Info(StatusMessage);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task RemoveSelectedAsync()
    {
        var selected = Apps.Where(a => a.IsSelected).ToList();
        if (selected.Count == 0)
        {
            StatusMessage = "Сначала выберите приложения для удаления";
            return;
        }

        IsLoading = true;
        IsError = false;
        int removed = 0;
        int failed = 0;

        foreach (var app in selected)
        {
            StatusMessage = $"Удаление {app.DisplayName}...";
            try
            {
                var success = await Task.Run(() => _debloat.RemoveApp(app.PackageName));
                if (success)
                {
                    removed++;
                    _log.Info($"Удалено: {app.DisplayName}");
                    System.Windows.Application.Current.Dispatcher.Invoke(() => Apps.Remove(app));
                }
                else
                {
                    failed++;
                    _log.Warn($"Не удалось удалить: {app.DisplayName}");
                }
            }
            catch (Exception ex)
            {
                failed++;
                _log.Error($"Ошибка удаления: {app.DisplayName}", ex);
            }
        }

        if (failed > 0)
        {
            IsError = true;
            StatusMessage = $"Удалено: {removed}, ошибок: {failed}";
        }
        else
        {
            StatusMessage = $"✓ Удалено приложений: {removed}";
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task CleanTempFilesAsync()
    {
        IsLoading = true;
        IsError = false;
        StatusMessage = "Очистка временных файлов...";

        try
        {
            var freed = await Task.Run(() => _debloat.CleanTempFiles());
            var freedMb = freed / 1024 / 1024;
            TempCleanResult = $"✓ Освобождено места на диске: {freedMb} МБ";
            StatusMessage = TempCleanResult;
            _log.Info(StatusMessage);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"❌ Ошибка очистки: {ex.Message}";
            _log.Error("CleanTempFiles", ex);
        }

        IsLoading = false;
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var app in Apps) app.IsSelected = true;
    }
}
