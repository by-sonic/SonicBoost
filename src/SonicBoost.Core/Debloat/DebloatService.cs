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
        new("Microsoft.BingWeather", "MSN Weather", "Weather app"),
        new("Microsoft.BingNews", "MSN News", "News app"),
        new("Microsoft.GetHelp", "Get Help", "Microsoft support app"),
        new("Microsoft.Getstarted", "Tips", "Windows tips app"),
        new("Microsoft.MicrosoftOfficeHub", "Office Hub", "Office promotion app"),
        new("Microsoft.MicrosoftSolitaireCollection", "Solitaire", "Solitaire games"),
        new("Microsoft.People", "People", "People contacts app"),
        new("Microsoft.WindowsFeedbackHub", "Feedback Hub", "Microsoft feedback"),
        new("Microsoft.Xbox.TCUI", "Xbox TCUI", "Xbox text/chat UI"),
        new("Microsoft.XboxApp", "Xbox App", "Xbox companion"),
        new("Microsoft.XboxSpeechToTextOverlay", "Xbox Speech", "Xbox speech overlay"),
        new("Microsoft.ZuneMusic", "Groove Music", "Music player"),
        new("Microsoft.ZuneVideo", "Movies & TV", "Video player"),
        new("Microsoft.WindowsMaps", "Windows Maps", "Maps app"),
        new("Microsoft.WindowsAlarms", "Alarms & Clock", "Clock and alarms"),
        new("Microsoft.YourPhone", "Phone Link", "Phone companion"),
        new("Microsoft.549981C3F5F10", "Cortana", "Cortana app"),
        new("Clipchamp.Clipchamp", "Clipchamp", "Video editor"),
        new("Microsoft.Todos", "Microsoft To Do", "Task management"),
        new("MicrosoftTeams", "Microsoft Teams", "Teams chat"),
        new("Microsoft.PowerAutomateDesktop", "Power Automate", "Automation tool"),
        new("Microsoft.MicrosoftStickyNotes", "Sticky Notes", "Sticky notes"),
        new("king.com.CandyCrushSaga", "Candy Crush Saga", "Game bloatware"),
        new("king.com.CandyCrushFriends", "Candy Crush Friends", "Game bloatware"),
        new("SpotifyAB.SpotifyMusic", "Spotify", "Spotify pre-install"),
        new("Disney.37853FC22B2CE", "Disney+", "Disney+ pre-install"),
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
