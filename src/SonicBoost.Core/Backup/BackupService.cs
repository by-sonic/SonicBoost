using Microsoft.Win32;
using System.Runtime.Versioning;
using System.Text.Json;

namespace SonicBoost.Core.Backup;

[SupportedOSPlatform("windows")]
public class BackupService
{
    private static readonly string BackupDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SonicBoost", "Backups");

    private readonly Dictionary<string, BackupEntry> _entries = new();

    public BackupService()
    {
        Directory.CreateDirectory(BackupDir);
    }

    public void BackupRegistryValue(string path, string key)
    {
        var id = $"{path}\\{key}";
        if (_entries.ContainsKey(id)) return;

        object? value = null;
        RegistryValueKind kind = RegistryValueKind.DWord;
        bool existed = false;

        try
        {
            using var regKey = OpenRegistryKey(path, false);
            if (regKey != null)
            {
                value = regKey.GetValue(key);
                if (value != null)
                {
                    kind = regKey.GetValueKind(key);
                    existed = true;
                }
            }
        }
        catch { }

        _entries[id] = new BackupEntry
        {
            RegistryPath = path,
            RegistryKey = key,
            OriginalValue = value,
            ValueKind = kind,
            Existed = existed,
            Timestamp = DateTime.UtcNow
        };
    }

    public void BackupServiceState(string serviceName, string startupType)
    {
        var id = $"SERVICE:{serviceName}";
        _entries[id] = new BackupEntry
        {
            ServiceName = serviceName,
            OriginalStartupType = startupType,
            Existed = true,
            Timestamp = DateTime.UtcNow
        };
    }

    public void RestoreAll()
    {
        foreach (var entry in _entries.Values)
        {
            try
            {
                if (!string.IsNullOrEmpty(entry.RegistryPath))
                    RestoreRegistryValue(entry);
                else if (!string.IsNullOrEmpty(entry.ServiceName))
                    RestoreServiceState(entry);
            }
            catch { }
        }
        _entries.Clear();
    }

    public void SaveToDisk()
    {
        var path = Path.Combine(BackupDir, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        var json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public int BackupCount => _entries.Count;

    private void RestoreRegistryValue(BackupEntry entry)
    {
        using var key = OpenRegistryKey(entry.RegistryPath!, true);
        if (key == null) return;

        if (!entry.Existed)
        {
            key.DeleteValue(entry.RegistryKey!, false);
        }
        else if (entry.OriginalValue != null)
        {
            key.SetValue(entry.RegistryKey!, entry.OriginalValue, entry.ValueKind);
        }
    }

    private void RestoreServiceState(BackupEntry entry)
    {
        if (string.IsNullOrEmpty(entry.ServiceName) || string.IsNullOrEmpty(entry.OriginalStartupType))
            return;

        var startMode = entry.OriginalStartupType switch
        {
            "Automatic" => "auto",
            "Manual" => "demand",
            "Disabled" => "disabled",
            _ => "demand"
        };

        using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "sc.exe",
            Arguments = $"config \"{entry.ServiceName}\" start= {startMode}",
            UseShellExecute = false,
            CreateNoWindow = true
        });
        process?.WaitForExit(5000);
    }

    private static RegistryKey? OpenRegistryKey(string path, bool writable)
    {
        if (path.StartsWith("HKLM\\") || path.StartsWith("HKEY_LOCAL_MACHINE\\"))
        {
            var subPath = path.Contains("HKLM\\") ? path[5..] : path[19..];
            return Registry.LocalMachine.OpenSubKey(subPath, writable);
        }
        if (path.StartsWith("HKCU\\") || path.StartsWith("HKEY_CURRENT_USER\\"))
        {
            var subPath = path.Contains("HKCU\\") ? path[5..] : path[18..];
            return Registry.CurrentUser.OpenSubKey(subPath, writable);
        }
        return null;
    }

    private class BackupEntry
    {
        public string? RegistryPath { get; set; }
        public string? RegistryKey { get; set; }
        public object? OriginalValue { get; set; }
        public RegistryValueKind ValueKind { get; set; }
        public string? ServiceName { get; set; }
        public string? OriginalStartupType { get; set; }
        public bool Existed { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
