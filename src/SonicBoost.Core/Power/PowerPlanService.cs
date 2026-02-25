using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;

namespace SonicBoost.Core.Power;

[SupportedOSPlatform("windows")]
public class PowerPlanService
{
    private static readonly string UltimatePerformanceGuid = "e9a42b02-d5df-448d-aa00-03f14749eb61";

    static PowerPlanService()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public PowerPlanInfo GetCurrentPlan()
    {
        var output = RunPowercfg("/getactivescheme");
        return ParsePlanInfo(output);
    }

    public List<PowerPlanInfo> GetAllPlans()
    {
        var output = RunPowercfg("/list");
        var plans = new List<PowerPlanInfo>();
        foreach (var line in output.Split('\n'))
        {
            if (line.Contains("GUID"))
            {
                plans.Add(ParsePlanInfo(line));
            }
        }
        return plans;
    }

    public bool IsUltimatePerformanceAvailable()
    {
        var plans = GetAllPlans();
        return plans.Any(p => p.Guid.Equals(UltimatePerformanceGuid, StringComparison.OrdinalIgnoreCase));
    }

    public void EnableUltimatePerformance()
    {
        if (!IsUltimatePerformanceAvailable())
        {
            RunPowercfg($"-duplicatescheme {UltimatePerformanceGuid}");
        }
        RunPowercfg($"/setactive {UltimatePerformanceGuid}");
    }

    public void SetPlan(string guid)
    {
        RunPowercfg($"/setactive {guid}");
    }

    public void SetHighPerformance()
    {
        RunPowercfg("/setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
    }

    public void DisableHibernation()
    {
        RunPowercfg("/hibernate off");
    }

    public void EnableHibernation()
    {
        RunPowercfg("/hibernate on");
    }

    private static Encoding GetOemEncoding()
    {
        try
        {
            var oemCp = CultureInfo.CurrentCulture.TextInfo.OEMCodePage;
            return Encoding.GetEncoding(oemCp);
        }
        catch
        {
            return Encoding.GetEncoding(866);
        }
    }

    private static string RunPowercfg(string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "powercfg",
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            StandardOutputEncoding = GetOemEncoding(),
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return "";
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit(5000);
        return output;
    }

    private static PowerPlanInfo ParsePlanInfo(string line)
    {
        var info = new PowerPlanInfo();
        var guidStart = line.IndexOf(':');
        if (guidStart >= 0)
        {
            var guidPart = line[(guidStart + 1)..].Trim();
            var spaceIdx = guidPart.IndexOf(' ');
            if (spaceIdx > 0)
            {
                info.Guid = guidPart[..spaceIdx].Trim();
                var nameStart = guidPart.IndexOf('(');
                var nameEnd = guidPart.IndexOf(')');
                if (nameStart >= 0 && nameEnd > nameStart)
                {
                    info.Name = guidPart[(nameStart + 1)..nameEnd];
                }
            }
        }
        info.IsActive = line.Contains('*');
        return info;
    }
}

public class PowerPlanInfo
{
    public string Guid { get; set; } = "";
    public string Name { get; set; } = "Unknown";
    public bool IsActive { get; set; }
}
