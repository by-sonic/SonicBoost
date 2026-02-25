using System.Collections.Concurrent;

namespace SonicBoost.Core.Logging;

public class LogService
{
    private static readonly string LogDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SonicBoost", "Logs");

    private readonly string _logFile;
    private readonly ConcurrentQueue<string> _recentEntries = new();
    private const int MaxRecentEntries = 200;

    public event Action<string>? OnLogEntry;

    public LogService()
    {
        Directory.CreateDirectory(LogDir);
        _logFile = Path.Combine(LogDir, $"sonic_{DateTime.Now:yyyy-MM-dd}.log");
    }

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);
    public void Error(string message) => Write("ERROR", message);
    public void Error(string message, Exception ex) => Write("ERROR", $"{message} | {ex.GetType().Name}: {ex.Message}");

    public string LogFilePath => _logFile;

    public IReadOnlyCollection<string> RecentEntries => _recentEntries.ToArray();

    private void Write(string level, string message)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}";

        _recentEntries.Enqueue(entry);
        while (_recentEntries.Count > MaxRecentEntries)
            _recentEntries.TryDequeue(out _);

        try
        {
            File.AppendAllText(_logFile, entry + Environment.NewLine);
        }
        catch
        {
            // Cannot write to log file
        }

        OnLogEntry?.Invoke(entry);
    }
}
