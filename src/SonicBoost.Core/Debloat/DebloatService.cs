using System.Diagnostics;
using System.Runtime.Versioning;

namespace SonicBoost.Core.Debloat;

[SupportedOSPlatform("windows")]
public class DebloatService
{
    public List<BloatApp> GetInstalledBloatware()
    {
        var known = GetKnownBloatware();
        var installed = new List<BloatApp>();

        try
        {
            var output = RunPowerShell("Get-AppxPackage | Select-Object -Property Name | ConvertTo-Json");
            if (string.IsNullOrWhiteSpace(output)) return installed;

            foreach (var app in known)
            {
                if (output.Contains(app.PackageName, StringComparison.OrdinalIgnoreCase))
                {
                    app.IsInstalled = true;
                    installed.Add(app);
                }
            }
        }
        catch { }

        return installed;
    }

    public bool RemoveApp(string packageName)
    {
        try
        {
            var output = RunPowerShell($"Get-AppxPackage *{packageName}* | Remove-AppxPackage -ErrorAction SilentlyContinue");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public long CleanTempFiles()
    {
        long totalFreed = 0;
        var dirs = new[]
        {
            Path.GetTempPath(),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"),
            @"C:\Windows\Temp",
            @"C:\Windows\SoftwareDistribution\Download",
        };

        foreach (var dir in dirs)
        {
            if (!Directory.Exists(dir)) continue;
            try
            {
                foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var info = new FileInfo(file);
                        totalFreed += info.Length;
                        info.Delete();
                    }
                    catch { }
                }
            }
            catch { }
        }

        return totalFreed;
    }

    private static string RunPowerShell(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -NonInteractive -Command \"{command}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return "";
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit(30000);
        return output;
    }

    private static List<BloatApp> GetKnownBloatware() =>
    [
        new("Microsoft.BingWeather", "Погода MSN", "Приложение погоды"),
        new("Microsoft.BingNews", "Новости MSN", "Новости"),
        new("Microsoft.GetHelp", "Справка", "Справка Microsoft"),
        new("Microsoft.Getstarted", "Советы", "Советы Windows"),
        new("Microsoft.MicrosoftOfficeHub", "Office Hub", "Рекламное приложение Office"),
        new("Microsoft.MicrosoftSolitaireCollection", "Солитер", "Игры пасьянса"),
        new("Microsoft.People", "Люди", "Контакты"),
        new("Microsoft.WindowsFeedbackHub", "Центр отзывов", "Отзывы Microsoft"),
        new("Microsoft.Xbox.TCUI", "Xbox TCUI", "Чат и текст Xbox"),
        new("Microsoft.XboxApp", "Xbox", "Приложение Xbox"),
        new("Microsoft.XboxSpeechToTextOverlay", "Xbox Речь", "Речевой ввод Xbox"),
        new("Microsoft.ZuneMusic", "Groove Music", "Музыка"),
        new("Microsoft.ZuneVideo", "Кино и ТВ", "Видеоплеер"),
        new("Microsoft.WindowsMaps", "Карты", "Карты"),
        new("Microsoft.WindowsAlarms", "Будильник и часы", "Часы и будильник"),
        new("Microsoft.YourPhone", "Ваш телефон", "Связь с телефоном"),
        new("Microsoft.549981C3F5F10", "Cortana", "Голосовой помощник"),
        new("Clipchamp.Clipchamp", "Clipchamp", "Видеоредактор"),
        new("Microsoft.Todos", "Microsoft To Do", "Задачи"),
        new("MicrosoftTeams", "Microsoft Teams", "Чат и звонки"),
        new("Microsoft.PowerAutomateDesktop", "Power Automate", "Автоматизация"),
        new("Microsoft.MicrosoftStickyNotes", "Заметки", "Закладки на рабочем столе"),
        new("king.com.CandyCrushSaga", "Candy Crush Saga", "Игра"),
        new("king.com.CandyCrushFriends", "Candy Crush Friends", "Игра"),
        new("SpotifyAB.SpotifyMusic", "Spotify", "Предустановленная музыка"),
        new("Disney.37853FC22B2CE", "Disney+", "Предустановленный Disney+"),
    ];
}

public class BloatApp
{
    public string PackageName { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public bool IsInstalled { get; set; }
    public bool IsSelected { get; set; }

    public BloatApp(string packageName, string displayName, string description)
    {
        PackageName = packageName;
        DisplayName = displayName;
        Description = description;
    }
}
