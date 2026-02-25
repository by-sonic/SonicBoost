using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks.Models;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.ServiceProcess;

namespace SonicBoost.Core.Services;

[SupportedOSPlatform("windows")]
public class ServiceManager
{
    private readonly BackupService _backup;

    public ServiceManager(BackupService backup)
    {
        _backup = backup;
    }

    public List<ServiceItem> GetOptimizableServices()
    {
        var definitions = GetServiceDefinitions();
        var result = new List<ServiceItem>();

        foreach (var def in definitions)
        {
            try
            {
                using var svc = new ServiceController(def.ServiceName);
                def.Status = svc.Status.ToString();
                def.IsRunning = svc.Status == ServiceControllerStatus.Running;
                def.StartupType = svc.StartType.ToString();
                def.IsDisabledByUser = svc.StartType == ServiceStartMode.Disabled;
                result.Add(def);
            }
            catch
            {
                // Service not found on this system
            }
        }

        return result;
    }

    public void DisableService(ServiceItem item)
    {
        _backup.BackupServiceState(item.ServiceName, item.StartupType);
        RunSc($"config \"{item.ServiceName}\" start= disabled");
        StopService(item.ServiceName);
    }

    public void EnableService(ServiceItem item, string startMode = "demand")
    {
        RunSc($"config \"{item.ServiceName}\" start= {startMode}");
    }

    public void StopService(string name)
    {
        RunSc($"stop \"{name}\"");
    }

    private static void RunSc(string args)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "sc.exe",
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        process?.WaitForExit(5000);
    }

    private static List<ServiceItem> GetServiceDefinitions() =>
    [
        new() { ServiceName = "SysMain", DisplayName = "SysMain (Superfetch)", Description = "Preloads apps into memory. Disabling saves RAM on SSD systems.", Category = "Performance", Risk = ServiceRisk.Safe },
        new() { ServiceName = "WSearch", DisplayName = "Windows Search", Description = "Indexes files for fast searching. Uses CPU and disk I/O.", Category = "Performance", Risk = ServiceRisk.Safe },
        new() { ServiceName = "DiagTrack", DisplayName = "Connected User Experiences", Description = "Telemetry service that sends data to Microsoft.", Category = "Privacy", Risk = ServiceRisk.Safe },
        new() { ServiceName = "dmwappushservice", DisplayName = "WAP Push Message Routing", Description = "Routes push messages for telemetry.", Category = "Privacy", Risk = ServiceRisk.Safe },
        new() { ServiceName = "MapsBroker", DisplayName = "Downloaded Maps Manager", Description = "Manages downloaded offline maps.", Category = "Bloat", Risk = ServiceRisk.Safe },
        new() { ServiceName = "Fax", DisplayName = "Fax", Description = "Enables sending and receiving faxes.", Category = "Bloat", Risk = ServiceRisk.Safe },
        new() { ServiceName = "Spooler", DisplayName = "Print Spooler", Description = "Manages print jobs. Disable if no printer.", Category = "Bloat", Risk = ServiceRisk.Caution },
        new() { ServiceName = "lfsvc", DisplayName = "Geolocation Service", Description = "Tracks device location.", Category = "Privacy", Risk = ServiceRisk.Safe },
        new() { ServiceName = "RetailDemo", DisplayName = "Retail Demo Service", Description = "Demo mode for retail stores.", Category = "Bloat", Risk = ServiceRisk.Safe },
        new() { ServiceName = "XblAuthManager", DisplayName = "Xbox Live Auth Manager", Description = "Xbox Live authentication. Disable if not using Xbox.", Category = "Gaming", Risk = ServiceRisk.Caution },
        new() { ServiceName = "XblGameSave", DisplayName = "Xbox Live Game Save", Description = "Syncs Xbox game saves to cloud.", Category = "Gaming", Risk = ServiceRisk.Caution },
        new() { ServiceName = "XboxGipSvc", DisplayName = "Xbox Accessory Management", Description = "Manages Xbox accessories.", Category = "Gaming", Risk = ServiceRisk.Caution },
        new() { ServiceName = "XboxNetApiSvc", DisplayName = "Xbox Live Networking", Description = "Xbox Live networking features.", Category = "Gaming", Risk = ServiceRisk.Caution },
        new() { ServiceName = "bthserv", DisplayName = "Bluetooth Support", Description = "Manages Bluetooth devices. Disable if not using BT.", Category = "Hardware", Risk = ServiceRisk.Caution },
        new() { ServiceName = "CDPUserSvc", DisplayName = "Connected Devices Platform", Description = "Cross-device experience features.", Category = "Bloat", Risk = ServiceRisk.Safe },
        new() { ServiceName = "PimIndexMaintenanceSvc", DisplayName = "Contact Data", Description = "Indexes contact data for fast lookup.", Category = "Bloat", Risk = ServiceRisk.Safe },
        new() { ServiceName = "WMPNetworkSvc", DisplayName = "Windows Media Player Network", Description = "Shares Windows Media Player library.", Category = "Bloat", Risk = ServiceRisk.Safe },
        new() { ServiceName = "WerSvc", DisplayName = "Windows Error Reporting", Description = "Sends crash reports to Microsoft.", Category = "Privacy", Risk = ServiceRisk.Safe },
    ];
}
