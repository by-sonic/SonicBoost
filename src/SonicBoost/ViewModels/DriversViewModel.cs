using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Drivers;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SonicBoost.ViewModels;

public partial class DriversViewModel : ObservableObject
{
    private readonly DriverService _drivers;

    [ObservableProperty] private bool _isLoading;

    public ObservableCollection<DriverRecommendation> Recommendations { get; } = [];

    public DriversViewModel(DriverService drivers)
    {
        _drivers = drivers;
    }

    [RelayCommand]
    private async Task LoadDriversAsync()
    {
        IsLoading = true;
        Recommendations.Clear();

        var items = await Task.Run(() => _drivers.GetRecommendations());
        foreach (var item in items) Recommendations.Add(item);

        IsLoading = false;
    }

    [RelayCommand]
    private static void OpenDownloadUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
