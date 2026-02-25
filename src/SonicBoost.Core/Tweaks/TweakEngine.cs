using Microsoft.Win32;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks.Models;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace SonicBoost.Core.Tweaks;

[SupportedOSPlatform("windows")]
public class TweakEngine
{
    private readonly BackupService _backup;

    public TweakEngine(BackupService backup)
    {
        _backup = backup;
    }

    public static bool IsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public List<TweakItem> GetAllTweaks() => TweakDefinitions.All();

    /// <summary>
    /// Applies tweak and verifies it was written. Throws on failure.
    /// </summary>
    public void ApplyTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.RegistryKey))
            throw new InvalidOperationException("Не задан путь или ключ реестра");

        if (tweak.RegistryPath.StartsWith("HKLM") && !IsAdmin())
            throw new UnauthorizedAccessException("Требуются права администратора для записи в HKLM");

        _backup.BackupRegistryValue(tweak.RegistryPath, tweak.RegistryKey);
        SetRegistryValue(tweak.RegistryPath, tweak.RegistryKey, tweak.EnabledValue!, tweak.ValueKind);

        if (!IsTweakApplied(tweak))
            throw new InvalidOperationException("Значение не записалось — проверьте права или политику безопасности");
    }

    /// <summary>
    /// Reverts tweak and verifies. Throws on failure.
    /// </summary>
    public void RevertTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.RegistryKey))
            throw new InvalidOperationException("Не задан путь или ключ реестра");

        if (tweak.DisabledValue != null)
        {
            SetRegistryValue(tweak.RegistryPath, tweak.RegistryKey, tweak.DisabledValue, tweak.ValueKind);

            var current = GetRegistryValue(tweak.RegistryPath, tweak.RegistryKey);
            if (current?.ToString() != tweak.DisabledValue.ToString())
                throw new InvalidOperationException("Откат не удался — значение не изменилось");
        }
    }

    public bool IsTweakApplied(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.RegistryKey))
            return false;

        try
        {
            var value = GetRegistryValue(tweak.RegistryPath, tweak.RegistryKey);
            if (value == null || tweak.EnabledValue == null)
                return false;
            return value.ToString() == tweak.EnabledValue.ToString();
        }
        catch
        {
            return false;
        }
    }

    private static void SetRegistryValue(string path, string key, object value, RegistryValueKind kind)
    {
        var (root, subPath) = ParseRegistryPath(path);
        using var regKey = root.CreateSubKey(subPath, true)
            ?? throw new UnauthorizedAccessException($"Не удалось открыть ключ: {path}");
        regKey.SetValue(key, value, kind);
    }

    private static object? GetRegistryValue(string path, string key)
    {
        var (root, subPath) = ParseRegistryPath(path);
        using var regKey = root.OpenSubKey(subPath, false);
        return regKey?.GetValue(key);
    }

    private static (RegistryKey root, string subPath) ParseRegistryPath(string path)
    {
        if (path.StartsWith("HKLM\\"))
            return (Registry.LocalMachine, path[5..]);
        if (path.StartsWith("HKCU\\"))
            return (Registry.CurrentUser, path[5..]);
        throw new ArgumentException($"Unsupported registry path: {path}");
    }
}
