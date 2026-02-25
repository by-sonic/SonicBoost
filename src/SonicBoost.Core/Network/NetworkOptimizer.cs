using Microsoft.Win32;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;

namespace SonicBoost.Core.Network;

[SupportedOSPlatform("windows")]
public class NetworkOptimizer
{
    private readonly BackupService _backup;

    public NetworkOptimizer(BackupService backup)
    {
        _backup = backup;
    }

    public List<TweakItem> GetNetworkTweaks() =>
    [
        new()
        {
            Id = "disable_nagle",
            Name = "Disable Nagle's Algorithm",
            Description = "Reduces network latency by sending packets immediately instead of buffering",
            Category = "Latency",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
            RegistryKey = "TcpNoDelay",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_tcp_timestamps",
            Name = "Disable TCP Timestamps",
            Description = "Disables TCP timestamps to reduce packet overhead",
            Category = "Latency",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
            RegistryKey = "Tcp1323Opts",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "network_throttling_off",
            Name = "Disable Network Throttling",
            Description = "Removes the 10-packet throttle limit on non-multimedia traffic",
            Category = "Throughput",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
            RegistryKey = "NetworkThrottlingIndex",
            EnabledValue = unchecked((int)0xFFFFFFFF),
            DisabledValue = 10,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_large_send_offload",
            Name = "Optimize TCP ACK Frequency",
            Description = "Increases TCP ACK frequency for lower latency in gaming",
            Category = "Latency",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
            RegistryKey = "TcpAckFrequency",
            EnabledValue = 1,
            DisabledValue = 2,
            ValueKind = RegistryValueKind.DWord
        },
    ];

    public void ApplyTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath)) return;
        _backup.BackupRegistryValue(tweak.RegistryPath, tweak.RegistryKey!);

        var (root, subPath) = ParsePath(tweak.RegistryPath);
        using var key = root.CreateSubKey(subPath, true);
        key?.SetValue(tweak.RegistryKey!, tweak.EnabledValue!, tweak.ValueKind);
    }

    public void RevertTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || tweak.DisabledValue == null) return;
        var (root, subPath) = ParsePath(tweak.RegistryPath);
        using var key = root.CreateSubKey(subPath, true);
        key?.SetValue(tweak.RegistryKey!, tweak.DisabledValue, tweak.ValueKind);
    }

    public void SetDns(string primary, string secondary)
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up
                    && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            foreach (var ni in interfaces)
            {
                var name = ni.Name;
                RunNetsh($"interface ip set dns \"{name}\" static {primary}");
                RunNetsh($"interface ip add dns \"{name}\" {secondary} index=2");
            }
        }
        catch { }
    }

    public void SetAutoTuningLevel(string level = "normal")
    {
        RunNetsh($"interface tcp set global autotuninglevel={level}");
    }

    private static void RunNetsh(string args)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        process?.WaitForExit(5000);
    }

    public bool IsTweakApplied(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath)) return false;
        try
        {
            var (root, subPath) = ParsePath(tweak.RegistryPath);
            using var key = root.OpenSubKey(subPath, false);
            var val = key?.GetValue(tweak.RegistryKey!);
            return val?.ToString() == tweak.EnabledValue?.ToString();
        }
        catch { return false; }
    }

    private static (RegistryKey root, string subPath) ParsePath(string path)
    {
        if (path.StartsWith("HKLM\\")) return (Registry.LocalMachine, path[5..]);
        if (path.StartsWith("HKCU\\")) return (Registry.CurrentUser, path[5..]);
        throw new ArgumentException($"Unsupported path: {path}");
    }
}
