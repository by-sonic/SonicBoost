using SonicBoost.Core.Hardware;
using System.Runtime.Versioning;

namespace SonicBoost.Core.Drivers;

[SupportedOSPlatform("windows")]
public class DriverService
{
    private readonly HardwareDetectionService _hw;

    public DriverService(HardwareDetectionService hw)
    {
        _hw = hw;
    }

    public List<DriverRecommendation> GetRecommendations()
    {
        var info = _hw.GetSystemInfo();
        var recommendations = new List<DriverRecommendation>();

        recommendations.Add(GetGpuDriver(info.Gpu));
        recommendations.Add(GetChipsetDriver(info.Motherboard));
        recommendations.AddRange(GetNetworkDrivers());
        recommendations.Add(GetAudioDriver());

        return recommendations.Where(r => r != null).ToList()!;
    }

    private static DriverRecommendation GetGpuDriver(GpuInfo gpu)
    {
        var rec = new DriverRecommendation
        {
            Category = "GPU",
            DeviceName = gpu.Name,
            CurrentVersion = gpu.DriverVersion,
        };

        switch (gpu.Vendor)
        {
            case GpuVendor.Nvidia:
                rec.Vendor = "NVIDIA";
                rec.DownloadUrl = "https://www.nvidia.com/Download/index.aspx";
                rec.Description = "Download the latest NVIDIA Game Ready or Studio driver";
                break;
            case GpuVendor.Amd:
                rec.Vendor = "AMD";
                rec.DownloadUrl = "https://www.amd.com/en/support";
                rec.Description = "Download the latest AMD Adrenalin driver";
                break;
            case GpuVendor.Intel:
                rec.Vendor = "Intel";
                rec.DownloadUrl = "https://www.intel.com/content/www/us/en/download-center/home.html";
                rec.Description = "Download the latest Intel Graphics driver";
                break;
            default:
                rec.Vendor = "Unknown";
                rec.DownloadUrl = "";
                rec.Description = "GPU vendor not recognized";
                break;
        }

        return rec;
    }

    private static DriverRecommendation GetChipsetDriver(MotherboardInfo mb)
    {
        var mfr = mb.Manufacturer.ToUpperInvariant();
        string url;
        string vendor;

        if (mfr.Contains("ASUS") || mfr.Contains("ASUSTEK"))
        {
            url = "https://www.asus.com/support/download-center/";
            vendor = "ASUS";
        }
        else if (mfr.Contains("MSI") || mfr.Contains("MICRO-STAR"))
        {
            url = "https://www.msi.com/support/download";
            vendor = "MSI";
        }
        else if (mfr.Contains("GIGABYTE"))
        {
            url = "https://www.gigabyte.com/Support";
            vendor = "Gigabyte";
        }
        else if (mfr.Contains("ASROCK"))
        {
            url = "https://www.asrock.com/support/index.asp";
            vendor = "ASRock";
        }
        else
        {
            url = "https://www.intel.com/content/www/us/en/download-center/home.html";
            vendor = mb.Manufacturer;
        }

        return new DriverRecommendation
        {
            Category = "Chipset",
            DeviceName = $"{mb.Manufacturer} {mb.Product}",
            Vendor = vendor,
            DownloadUrl = url,
            Description = $"Download chipset drivers from {vendor} support page"
        };
    }

    private static List<DriverRecommendation> GetNetworkDrivers()
    {
        var list = new List<DriverRecommendation>();
        try
        {
            using var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled=True");
            foreach (var obj in searcher.Get())
            {
                var name = obj["Name"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(name)) continue;

                var upper = name.ToUpperInvariant();
                string url;
                string vendor;

                if (upper.Contains("REALTEK"))
                {
                    url = "https://www.realtek.com/en/downloads";
                    vendor = "Realtek";
                }
                else if (upper.Contains("INTEL"))
                {
                    url = "https://www.intel.com/content/www/us/en/download-center/home.html";
                    vendor = "Intel";
                }
                else if (upper.Contains("KILLER") || upper.Contains("QUALCOMM"))
                {
                    url = "https://www.intel.com/content/www/us/en/download-center/home.html";
                    vendor = "Killer/Intel";
                }
                else
                {
                    continue;
                }

                list.Add(new DriverRecommendation
                {
                    Category = "Network",
                    DeviceName = name,
                    Vendor = vendor,
                    DownloadUrl = url,
                    Description = $"Download the latest {vendor} network driver"
                });
            }
        }
        catch { }
        return list;
    }

    private static DriverRecommendation GetAudioDriver()
    {
        string deviceName = "Audio Device";
        string url = "https://www.realtek.com/en/downloads";
        string vendor = "Realtek";

        try
        {
            using var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice");
            foreach (var obj in searcher.Get())
            {
                var name = obj["Name"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(name))
                {
                    deviceName = name;
                    var upper = name.ToUpperInvariant();
                    if (upper.Contains("NVIDIA"))
                    {
                        url = "https://www.nvidia.com/Download/index.aspx";
                        vendor = "NVIDIA";
                    }
                    else if (upper.Contains("AMD"))
                    {
                        url = "https://www.amd.com/en/support";
                        vendor = "AMD";
                    }
                    break;
                }
            }
        }
        catch { }

        return new DriverRecommendation
        {
            Category = "Audio",
            DeviceName = deviceName,
            Vendor = vendor,
            DownloadUrl = url,
            Description = $"Download the latest {vendor} audio driver"
        };
    }
}

public class DriverRecommendation
{
    public string Category { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string Vendor { get; set; } = "";
    public string CurrentVersion { get; set; } = "";
    public string DownloadUrl { get; set; } = "";
    public string Description { get; set; } = "";
}
