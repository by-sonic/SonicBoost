using System.Management;
using System.Runtime.Versioning;

namespace SonicBoost.Core.Hardware;

[SupportedOSPlatform("windows")]
public class HardwareDetectionService
{
    public HardwareInfo GetSystemInfo()
    {
        return new HardwareInfo
        {
            Cpu = GetCpuInfo(),
            Gpu = GetGpuInfo(),
            Ram = GetRamInfo(),
            Motherboard = GetMotherboardInfo(),
            Os = GetOsInfo(),
            Drives = GetDriveInfo()
        };
    }

    private CpuInfo GetCpuInfo()
    {
        var info = new CpuInfo();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                info.Name = obj["Name"]?.ToString()?.Trim() ?? "Unknown";
                info.Cores = Convert.ToInt32(obj["NumberOfCores"]);
                info.Threads = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                info.MaxClockSpeed = Convert.ToInt32(obj["MaxClockSpeed"]);
                break;
            }
        }
        catch { }
        return info;
    }

    private GpuInfo GetGpuInfo()
    {
        var info = new GpuInfo();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var obj in searcher.Get())
            {
                info.Name = obj["Name"]?.ToString() ?? "Unknown";
                info.DriverVersion = obj["DriverVersion"]?.ToString() ?? "Unknown";
                info.AdapterRam = Convert.ToInt64(obj["AdapterRAM"]);
                info.VideoProcessor = obj["VideoProcessor"]?.ToString() ?? "";
                info.PnpDeviceId = obj["PNPDeviceID"]?.ToString() ?? "";

                var name = info.Name.ToUpperInvariant();
                if (name.Contains("NVIDIA") || name.Contains("GEFORCE") || name.Contains("RTX") || name.Contains("GTX"))
                    info.Vendor = GpuVendor.Nvidia;
                else if (name.Contains("AMD") || name.Contains("RADEON"))
                    info.Vendor = GpuVendor.Amd;
                else if (name.Contains("INTEL") || name.Contains("ARC"))
                    info.Vendor = GpuVendor.Intel;

                if (info.Vendor != GpuVendor.Unknown)
                    break;
            }
        }
        catch { }
        return info;
    }

    private RamInfo GetRamInfo()
    {
        var info = new RamInfo();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            long total = 0;
            foreach (var obj in searcher.Get())
            {
                total += Convert.ToInt64(obj["Capacity"]);
                info.Speed = Convert.ToInt32(obj["Speed"]);
                info.Modules++;
            }
            info.TotalBytes = total;
        }
        catch { }
        return info;
    }

    private MotherboardInfo GetMotherboardInfo()
    {
        var info = new MotherboardInfo();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (var obj in searcher.Get())
            {
                info.Manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                info.Product = obj["Product"]?.ToString() ?? "Unknown";
                break;
            }
        }
        catch { }
        return info;
    }

    private OsInfo GetOsInfo()
    {
        var info = new OsInfo();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (var obj in searcher.Get())
            {
                info.Name = obj["Caption"]?.ToString() ?? "Unknown";
                info.Version = obj["Version"]?.ToString() ?? "Unknown";
                info.BuildNumber = obj["BuildNumber"]?.ToString() ?? "Unknown";
                info.Architecture = obj["OSArchitecture"]?.ToString() ?? "Unknown";
                break;
            }
        }
        catch { }
        return info;
    }

    private List<DriveInfo_> GetDriveInfo()
    {
        var drives = new List<DriveInfo_>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (var obj in searcher.Get())
            {
                drives.Add(new DriveInfo_
                {
                    Model = obj["Model"]?.ToString() ?? "Unknown",
                    SizeBytes = Convert.ToInt64(obj["Size"] ?? 0),
                    MediaType = obj["MediaType"]?.ToString() ?? "Unknown",
                    InterfaceType = obj["InterfaceType"]?.ToString() ?? "Unknown"
                });
            }
        }
        catch { }
        return drives;
    }
}

public class HardwareInfo
{
    public CpuInfo Cpu { get; set; } = new();
    public GpuInfo Gpu { get; set; } = new();
    public RamInfo Ram { get; set; } = new();
    public MotherboardInfo Motherboard { get; set; } = new();
    public OsInfo Os { get; set; } = new();
    public List<DriveInfo_> Drives { get; set; } = [];
}

public class CpuInfo
{
    public string Name { get; set; } = "Unknown";
    public int Cores { get; set; }
    public int Threads { get; set; }
    public int MaxClockSpeed { get; set; }
}

public class GpuInfo
{
    public string Name { get; set; } = "Unknown";
    public string DriverVersion { get; set; } = "Unknown";
    public long AdapterRam { get; set; }
    public string VideoProcessor { get; set; } = "";
    public string PnpDeviceId { get; set; } = "";
    public GpuVendor Vendor { get; set; } = GpuVendor.Unknown;
    public string RamFormatted => AdapterRam > 0 ? $"{AdapterRam / 1024 / 1024 / 1024} GB" : "N/A";
}

public enum GpuVendor { Unknown, Nvidia, Amd, Intel }

public class RamInfo
{
    public long TotalBytes { get; set; }
    public int Speed { get; set; }
    public int Modules { get; set; }
    public string TotalFormatted => $"{TotalBytes / 1024 / 1024 / 1024} GB";
}

public class MotherboardInfo
{
    public string Manufacturer { get; set; } = "Unknown";
    public string Product { get; set; } = "Unknown";
}

public class OsInfo
{
    public string Name { get; set; } = "Unknown";
    public string Version { get; set; } = "Unknown";
    public string BuildNumber { get; set; } = "Unknown";
    public string Architecture { get; set; } = "Unknown";
}

public class DriveInfo_
{
    public string Model { get; set; } = "Unknown";
    public long SizeBytes { get; set; }
    public string MediaType { get; set; } = "Unknown";
    public string InterfaceType { get; set; } = "Unknown";
    public string SizeFormatted => $"{SizeBytes / 1024 / 1024 / 1024} GB";
}
