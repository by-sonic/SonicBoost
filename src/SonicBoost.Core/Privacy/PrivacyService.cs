using Microsoft.Win32;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks;
using SonicBoost.Core.Tweaks.Models;
using System.Runtime.Versioning;

namespace SonicBoost.Core.Privacy;

[SupportedOSPlatform("windows")]
public class PrivacyService
{
    private readonly BackupService _backup;

    public PrivacyService(BackupService backup)
    {
        _backup = backup;
    }

    public List<TweakItem> GetPrivacyTweaks() =>
    [
        new()
        {
            Id = "disable_telemetry",
            Name = "Отключить телеметрию",
            Description = "Устанавливает уровень сбора телеметрии на минимум (только безопасность)",
            Category = "Телеметрия",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection",
            RegistryKey = "AllowTelemetry",
            EnabledValue = 0,
            DisabledValue = 3,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_advertising_id",
            Name = "Отключить рекламный идентификатор",
            Description = "Запрещает приложениям использовать рекламный ID для таргета",
            Category = "Конфиденциальность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
            RegistryKey = "Enabled",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_activity_history",
            Name = "Отключить журнал активности",
            Description = "Запрещает Windows собирать историю активности",
            Category = "Конфиденциальность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\System",
            RegistryKey = "EnableActivityFeed",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_activity_upload",
            Name = "Отключить отправку журнала активности",
            Description = "Запрещает отправку истории активности в Microsoft",
            Category = "Конфиденциальность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\System",
            RegistryKey = "UploadUserActivities",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_feedback",
            Name = "Отключить запросы отзывов",
            Description = "Запрещает Windows запрашивать отзывы",
            Category = "Конфиденциальность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Siuf\Rules",
            RegistryKey = "NumberOfSIUFInPeriod",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_location_tracking",
            Name = "Отключить отслеживание местоположения",
            Description = "Отключает системную службу геолокации",
            Category = "Конфиденциальность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors",
            RegistryKey = "DisableLocation",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_app_diagnostics",
            Name = "Отключить диагностику приложений",
            Description = "Запрещает приложениям доступ к диагностическим данным",
            Category = "Конфиденциальность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{2297E4E2-5DBE-466D-A12B-0F8286F0D9CA}",
            RegistryKey = "Value",
            EnabledValue = "Deny",
            DisabledValue = "Allow",
            ValueKind = RegistryValueKind.String
        },
        new()
        {
            Id = "disable_copilot",
            Name = "Отключить Windows Copilot",
            Description = "Отключает помощника Copilot (Win11 23H2+)",
            Category = "ИИ",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Software\Policies\Microsoft\Windows\WindowsCopilot",
            RegistryKey = "TurnOffWindowsCopilot",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_recall",
            Name = "Отключить Windows Recall",
            Description = "Отключает функцию снимков экрана Recall",
            Category = "ИИ",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Software\Policies\Microsoft\Windows\WindowsAI",
            RegistryKey = "DisableAIDataAnalysis",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
    ];

    public void ApplyTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath))
            throw new InvalidOperationException("Не задан путь реестра");

        if (tweak.RegistryPath.StartsWith("HKLM") && !Tweaks.TweakEngine.IsAdmin())
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

    public bool IsTweakApplied(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath)) return false;
        try
        {
            var (root, subPath) = ParsePath(tweak.RegistryPath);
            using var key = root.OpenSubKey(subPath, false);
            var value = key?.GetValue(tweak.RegistryKey!);
            return value?.ToString() == tweak.EnabledValue?.ToString();
        }
        catch { return false; }
    }

    public void BlockTelemetryHosts()
    {
        var hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
        var telemetryHosts = new[]
        {
            "vortex.data.microsoft.com",
            "vortex-win.data.microsoft.com",
            "telecommand.telemetry.microsoft.com",
            "telecommand.telemetry.microsoft.com.nsatc.net",
            "oca.telemetry.microsoft.com",
            "oca.telemetry.microsoft.com.nsatc.net",
            "sqm.telemetry.microsoft.com",
            "sqm.telemetry.microsoft.com.nsatc.net",
            "watson.telemetry.microsoft.com",
            "watson.telemetry.microsoft.com.nsatc.net",
            "redir.metaservices.microsoft.com",
            "choice.microsoft.com",
            "choice.microsoft.com.nsatc.net",
            "settings-sandbox.data.microsoft.com",
        };

        var existingContent = File.Exists(hostsPath) ? File.ReadAllText(hostsPath) : "";
        var linesToAdd = new List<string>();

        if (!existingContent.Contains("# SonicBoost Telemetry Block"))
            linesToAdd.Add("\n# SonicBoost Telemetry Block");

        foreach (var host in telemetryHosts)
        {
            if (!existingContent.Contains(host))
                linesToAdd.Add($"0.0.0.0 {host}");
        }

        if (linesToAdd.Count > 0)
        {
            linesToAdd.Add("# End SonicBoost Block\n");
            File.AppendAllLines(hostsPath, linesToAdd);
        }
    }

    private static (RegistryKey root, string subPath) ParsePath(string path)
    {
        if (path.StartsWith("HKLM\\"))
            return (Registry.LocalMachine, path[5..]);
        if (path.StartsWith("HKCU\\"))
            return (Registry.CurrentUser, path[5..]);
        throw new ArgumentException($"Unsupported path: {path}");
    }
}
