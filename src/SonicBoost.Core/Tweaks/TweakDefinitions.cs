using Microsoft.Win32;
using SonicBoost.Core.Tweaks.Models;

namespace SonicBoost.Core.Tweaks;

public static class TweakDefinitions
{
    public static List<TweakItem> All() =>
    [
        new()
        {
            Id = "disable_game_bar",
            Name = "Disable Game Bar",
            Description = "Disables Xbox Game Bar overlay which consumes resources during gaming",
            Category = "Gaming",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR",
            RegistryKey = "AppCaptureEnabled",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_game_dvr",
            Name = "Disable Game DVR",
            Description = "Disables background recording and Game DVR features",
            Category = "Gaming",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR",
            RegistryKey = "AllowGameDVR",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_fullscreen_optimizations",
            Name = "Disable Fullscreen Optimizations",
            Description = "Globally disables fullscreen optimizations that can cause input lag",
            Category = "Gaming",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\System\GameConfigStore",
            RegistryKey = "GameDVR_FSEBehaviorMode",
            EnabledValue = 2,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "hardware_gpu_scheduling",
            Name = "Hardware-Accelerated GPU Scheduling",
            Description = "Enables HAGS to reduce latency and improve performance (requires restart)",
            Category = "Gaming",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers",
            RegistryKey = "HwSchMode",
            EnabledValue = 2,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "game_mode",
            Name = "Enable Game Mode",
            Description = "Enables Windows Game Mode for better resource allocation during gaming",
            Category = "Gaming",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\GameBar",
            RegistryKey = "AutoGameModeEnabled",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_mouse_acceleration",
            Name = "Disable Mouse Acceleration",
            Description = "Disables pointer precision enhancement for consistent mouse movement",
            Category = "Gaming",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Control Panel\Mouse",
            RegistryKey = "MouseSpeed",
            EnabledValue = "0",
            DisabledValue = "1",
            ValueKind = RegistryValueKind.String
        },
        new()
        {
            Id = "disable_visual_effects",
            Name = "Disable Visual Effects",
            Description = "Sets visual effects to best performance mode, freeing GPU/CPU resources",
            Category = "Performance",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects",
            RegistryKey = "VisualFXSetting",
            EnabledValue = 2,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_transparency",
            Name = "Disable Transparency Effects",
            Description = "Disables window transparency effects to improve rendering performance",
            Category = "Performance",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
            RegistryKey = "EnableTransparency",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_animations",
            Name = "Disable Window Animations",
            Description = "Disables minimize/maximize animations for snappier feel",
            Category = "Performance",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Control Panel\Desktop\WindowMetrics",
            RegistryKey = "MinAnimate",
            EnabledValue = "0",
            DisabledValue = "1",
            ValueKind = RegistryValueKind.String
        },
        new()
        {
            Id = "disable_cortana",
            Name = "Disable Cortana",
            Description = "Disables Cortana to free up memory and CPU usage",
            Category = "Performance",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
            RegistryKey = "AllowCortana",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_web_search",
            Name = "Disable Web Search in Start Menu",
            Description = "Disables Bing web results in the Start Menu search",
            Category = "Performance",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Policies\Microsoft\Windows\Explorer",
            RegistryKey = "DisableSearchBoxSuggestions",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_widgets",
            Name = "Disable Widgets",
            Description = "Disables the Widgets panel (Win11) to save resources",
            Category = "Performance",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Dsh",
            RegistryKey = "AllowNewsAndInterests",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "gpu_priority",
            Name = "GPU Priority for Gaming",
            Description = "Sets high GPU priority for foreground applications",
            Category = "Gaming",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games",
            RegistryKey = "GPU Priority",
            EnabledValue = 8,
            DisabledValue = 2,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "cpu_priority_games",
            Name = "High CPU Priority for Games",
            Description = "Sets foreground game processes to high scheduling priority",
            Category = "Gaming",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games",
            RegistryKey = "Priority",
            EnabledValue = 6,
            DisabledValue = 2,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_power_throttling",
            Name = "Disable Power Throttling",
            Description = "Prevents Windows from throttling CPU for power savings",
            Category = "Performance",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling",
            RegistryKey = "PowerThrottlingOff",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "network_throttling",
            Name = "Disable Network Throttling",
            Description = "Removes the 10ms network throttling delay for games and multimedia",
            Category = "Network",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
            RegistryKey = "NetworkThrottlingIndex",
            EnabledValue = unchecked((int)0xFFFFFFFF),
            DisabledValue = 10,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "high_timer_resolution",
            Name = "High System Timer Resolution",
            Description = "Enables global high timer resolution for smoother frame pacing",
            Category = "Gaming",
            Risk = TweakRisk.Advanced,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\kernel",
            RegistryKey = "GlobalTimerResolutionRequests",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_prefetch",
            Name = "Disable Prefetch",
            Description = "Disables Windows prefetch on SSD systems to reduce disk writes",
            Category = "Performance",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters",
            RegistryKey = "EnablePrefetcher",
            EnabledValue = 0,
            DisabledValue = 3,
            ValueKind = RegistryValueKind.DWord
        },
    ];
}
