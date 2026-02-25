using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonicBoost.Core.Hardware;

namespace SonicBoost.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly HardwareDetectionService _hw;

    [ObservableProperty] private string _cpuName = "Определение...";
    [ObservableProperty] private string _cpuDetails = "";
    [ObservableProperty] private string _gpuName = "Определение...";
    [ObservableProperty] private string _gpuDetails = "";
    [ObservableProperty] private string _ramInfo = "Определение...";
    [ObservableProperty] private string _motherboard = "Определение...";
    [ObservableProperty] private string _osInfo = "Определение...";
    [ObservableProperty] private string _storageInfo = "Определение...";
    [ObservableProperty] private int _optimizationScore;
    [ObservableProperty] private bool _isLoading = true;

    public DashboardViewModel(HardwareDetectionService hw)
    {
        _hw = hw;
    }

    [RelayCommand]
    private async Task LoadSystemInfoAsync()
    {
        IsLoading = true;
        await Task.Run(() =>
        {
            var info = _hw.GetSystemInfo();

            CpuName = info.Cpu.Name;
            CpuDetails = $"{info.Cpu.Cores} ядер / {info.Cpu.Threads} потоков @ {info.Cpu.MaxClockSpeed} МГц";

            GpuName = info.Gpu.Name;
            GpuDetails = $"VRAM: {info.Gpu.RamFormatted} | Драйвер: {info.Gpu.DriverVersion}";

            RamInfo = $"{info.Ram.TotalFormatted} ({info.Ram.Modules} модулей @ {info.Ram.Speed} МГц)";
            Motherboard = $"{info.Motherboard.Manufacturer} {info.Motherboard.Product}";
            OsInfo = $"{info.Os.Name} ({info.Os.Architecture}), сборка {info.Os.BuildNumber}";

            if (info.Drives.Count > 0)
            {
                var driveTexts = info.Drives.Select(d => $"{d.Model} ({d.SizeFormatted})");
                StorageInfo = string.Join(" | ", driveTexts);
            }

            OptimizationScore = CalculateScore();
        });
        IsLoading = false;
    }

    private static int CalculateScore()
    {
        int score = 50;
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR");
            if (key?.GetValue("AppCaptureEnabled") is int val && val == 0) score += 5;
        }
        catch { }

        try
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection");
            if (key?.GetValue("AllowTelemetry") is int val && val == 0) score += 10;
        }
        catch { }

        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("EnableTransparency") is int val && val == 0) score += 5;
        }
        catch { }

        return Math.Min(score, 100);
    }
}
