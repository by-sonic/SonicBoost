using Microsoft.Win32;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks.Models;
using System.Runtime.Versioning;

namespace SonicBoost.Core.Tweaks;

[SupportedOSPlatform("windows")]
public class TweakEngine
{
    private readonly BackupService _backup;

    public TweakEngine(BackupService backup)
    {
        _backup = backup;
    }

    public List<TweakItem> GetAllTweaks() => TweakDefinitions.All();

    public void ApplyTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.RegistryKey))
            return;

        _backup.BackupRegistryValue(tweak.RegistryPath, tweak.RegistryKey);
        SetRegistryValue(tweak.RegistryPath, tweak.RegistryKey, tweak.EnabledValue!, tweak.ValueKind);
    }

    public void RevertTweak(TweakItem tweak)
    {
        if (string.IsNullOrEmpty(tweak.RegistryPath) || string.IsNullOrEmpty(tweak.RegistryKey))
            return;

        if (tweak.DisabledValue != null)
            SetRegistryValue(tweak.RegistryPath, tweak.RegistryKey, tweak.DisabledValue, tweak.ValueKind);
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
        using var regKey = root.CreateSubKey(subPath, true);
        regKey?.SetValue(key, value, kind);
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
