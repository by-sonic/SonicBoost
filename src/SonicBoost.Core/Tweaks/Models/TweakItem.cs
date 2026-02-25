using CommunityToolkit.Mvvm.ComponentModel;

namespace SonicBoost.Core.Tweaks.Models;

public partial class TweakItem : ObservableObject
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public TweakRisk Risk { get; init; } = TweakRisk.Safe;

    public string? RegistryPath { get; init; }
    public string? RegistryKey { get; init; }
    public object? EnabledValue { get; init; }
    public object? DisabledValue { get; init; }
    public Microsoft.Win32.RegistryValueKind ValueKind { get; init; } = Microsoft.Win32.RegistryValueKind.DWord;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private bool _isApplying;
}

public enum TweakRisk
{
    Safe,
    Moderate,
    Advanced
}
