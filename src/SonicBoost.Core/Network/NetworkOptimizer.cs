using Microsoft.Win32;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks;
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
            Name = "Отключить алгоритм Нейгла",
            Description = "Снижает задержку за счёт немедленной отправки пакетов",
            Category = "Задержка",
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
            Name = "Отключить TCP-метки времени",
            Description = "Отключает метки времени TCP для уменьшения накладных расходов",
            Category = "Задержка",
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
            Name = "Отключить сетевой троттлинг",
            Description = "Снимает лимит в 10 пакетов для немультимедийного трафика",
            Category = "Пропускная способность",
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
            Name = "Оптимизировать частоту TCP ACK",
            Description = "Увеличивает частоту подтверждений TCP для снижения задержки в играх",
            Category = "Задержка",
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
        if (string.IsNullOrEmpty(tweak.RegistryPath))
            throw new InvalidOperationException("Не задан путь реестра");

        if (tweak.RegistryPath.StartsWith("HKLM") && !TweakEngine.IsAdmin())
            throw new UnauthorizedAccessException("Требуются права администратора для записи в HKLM");

        _backup.BackupRegistryValue(tweak.RegistryPath, tweak.RegistryKey!);

        var (root, subPath) = ParsePath(tweak.RegistryPath);
        using var key = root.CreateSubKey(subPath, true)
            ?? throw new UnauthorizedAccessException($"Не удалось открыть ключ: {tweak.RegistryPath}");
        key.SetValue(tweak.RegistryKey!, tweak.EnabledValue!, tweak.ValueKind);

        if (!IsTweakApplied(tweak))
            throw new InvalidOperationException("Значение не записалось — проверьте права или политику безопасности");
    }

    public void RevertTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || tweak.DisabledValue == null)
            throw new InvalidOperationException("Не задан путь реестра или значение по умолчанию");

        var (root, subPath) = ParsePath(tweak.RegistryPath);
        using var key = root.CreateSubKey(subPath, true)
            ?? throw new UnauthorizedAccessException($"Не удалось открыть ключ: {tweak.RegistryPath}");
        key.SetValue(tweak.RegistryKey!, tweak.DisabledValue, tweak.ValueKind);
    }

    public (bool success, string output) SetDns(string primary, string secondary)
    {
        var results = new List<string>();
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up
                    && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToList();

            if (interfaces.Count == 0)
                return (false, "Нет активных сетевых интерфейсов");

            foreach (var ni in interfaces)
            {
                var name = ni.Name;
                var r1 = RunNetsh($"interface ip set dns \"{name}\" static {primary}");
                var r2 = RunNetsh($"interface ip add dns \"{name}\" {secondary} index=2");
                results.Add($"{name}: {r1.output.Trim()} / {r2.output.Trim()}");
            }
            return (true, string.Join("; ", results));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public void SetAutoTuningLevel(string level = "normal")
    {
        RunNetsh($"interface tcp set global autotuninglevel={level}");
    }

    private static (int exitCode, string output) RunNetsh(string args)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        process.Start();
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit(5000);
        return (process.ExitCode, string.IsNullOrEmpty(stdout) ? stderr : stdout);
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
