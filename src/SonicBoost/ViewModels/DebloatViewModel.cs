using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Debloat;
using System.Collections.ObjectModel;

namespace SonicBoost.ViewModels;

public partial class DebloatViewModel : ObservableObject
{
    private readonly DebloatService _debloat;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private string _tempCleanResult = "";

    public ObservableCollection<BloatApp> Apps { get; } = [];

    public DebloatViewModel(DebloatService debloat)
    {
        _debloat = debloat;
    }

    [RelayCommand]
    private async Task LoadAppsAsync()
    {
        IsLoading = true;
        Apps.Clear();
        StatusMessage = "Сканирование установленных приложений...";

        var items = await Task.Run(() => _debloat.GetInstalledBloatware());
        foreach (var item in items) Apps.Add(item);

        StatusMessage = $"Найдено приложений для удаления: {items.Count}";
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
        int removed = 0;
        foreach (var app in selected)
        {
            StatusMessage = $"Удаление {app.DisplayName}...";
            var success = await Task.Run(() => _debloat.RemoveApp(app.PackageName));
            if (success)
            {
                removed++;
                System.Windows.Application.Current.Dispatcher.Invoke(() => Apps.Remove(app));
            }
        }
        StatusMessage = $"Удалено приложений: {removed}";
        IsLoading = false;
    }

    [RelayCommand]
    private async Task CleanTempFilesAsync()
    {
        IsLoading = true;
        StatusMessage = "Очистка временных файлов...";

        var freed = await Task.Run(() => _debloat.CleanTempFiles());
        var freedMb = freed / 1024 / 1024;
        TempCleanResult = $"Освобождено места на диске: {freedMb} МБ";
        StatusMessage = TempCleanResult;
        IsLoading = false;
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var app in Apps) app.IsSelected = true;
    }
}
