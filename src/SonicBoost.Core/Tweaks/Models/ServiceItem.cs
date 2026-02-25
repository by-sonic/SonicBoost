using CommunityToolkit.Mvvm.ComponentModel;

namespace SonicBoost.Core.Tweaks.Models;

public partial class ServiceItem : ObservableObject
{
    public string ServiceName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public ServiceRisk Risk { get; init; } = ServiceRisk.Safe;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _status = "Unknown";

    [ObservableProperty]
    private string _startupType = "Automatic";

    [ObservableProperty]
    private bool _isDisabledByUser;
}

public enum ServiceRisk
{
    Safe,
    Caution,
    Risky
}
