using SonicBoost.Core.Backup;
using SonicBoost.Core.Tweaks;
using SonicBoost.Core.Tweaks.Models;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.ServiceProcess;
using System.Text;

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
        if (!TweakEngine.IsAdmin())
            throw new UnauthorizedAccessException("Требуются права администратора для управления службами");

        _backup.BackupServiceState(item.ServiceName, item.StartupType);
        var (exitCode, output) = RunScWithOutput($"config \"{item.ServiceName}\" start= disabled");
        if (exitCode != 0)
            throw new InvalidOperationException($"sc.exe вернул код {exitCode}: {output}");

        StopService(item.ServiceName);

        using var svc = new ServiceController(item.ServiceName);
        svc.Refresh();
        if (svc.StartType != ServiceStartMode.Disabled)
            throw new InvalidOperationException($"Служба {item.DisplayName} не была отключена — проверьте политику безопасности");
    }

    public void EnableService(ServiceItem item, string startMode = "demand")
    {
        if (!TweakEngine.IsAdmin())
            throw new UnauthorizedAccessException("Требуются права администратора для управления службами");

        var (exitCode, output) = RunScWithOutput($"config \"{item.ServiceName}\" start= {startMode}");
        if (exitCode != 0)
            throw new InvalidOperationException($"sc.exe вернул код {exitCode}: {output}");
    }

    public void StopService(string name)
    {
        RunScWithOutput($"stop \"{name}\"");
    }

    private static Encoding GetOemEncoding()
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
        }
        catch { return Encoding.GetEncoding(866); }
    }

    private static (int exitCode, string output) RunScWithOutput(string args)
    {
        var encoding = GetOemEncoding();
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "sc.exe",
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = encoding,
            StandardErrorEncoding = encoding
        };
        process.Start();
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit(5000);
        return (process.ExitCode, string.IsNullOrEmpty(stdout) ? stderr : stdout);
    }

    private static List<ServiceItem> GetServiceDefinitions() =>
    [
        new() { ServiceName = "SysMain", DisplayName = "SysMain (Superfetch)", Description = "Предзагрузка приложений в память. Отключение экономит ОЗУ на SSD.", Category = "Производительность", Risk = ServiceRisk.Safe },
        new() { ServiceName = "WSearch", DisplayName = "Поиск Windows", Description = "Индексация файлов для поиска. Нагружает CPU и диск.", Category = "Производительность", Risk = ServiceRisk.Safe },
        new() { ServiceName = "DiagTrack", DisplayName = "Данные о подключённых пользователях", Description = "Телеметрия, отправка данных в Microsoft.", Category = "Конфиденциальность", Risk = ServiceRisk.Safe },
        new() { ServiceName = "dmwappushservice", DisplayName = "Маршрутизация WAP Push", Description = "Маршрутизация push-сообщений для телеметрии.", Category = "Конфиденциальность", Risk = ServiceRisk.Safe },
        new() { ServiceName = "MapsBroker", DisplayName = "Диспетчер карт", Description = "Управление офлайн-картами.", Category = "Дополнительно", Risk = ServiceRisk.Safe },
        new() { ServiceName = "Fax", DisplayName = "Факс", Description = "Отправка и приём факсов.", Category = "Дополнительно", Risk = ServiceRisk.Safe },
        new() { ServiceName = "Spooler", DisplayName = "Диспетчер печати", Description = "Управление заданиями печати. Отключите, если нет принтера.", Category = "Дополнительно", Risk = ServiceRisk.Caution },
        new() { ServiceName = "lfsvc", DisplayName = "Служба геолокации", Description = "Отслеживание местоположения устройства.", Category = "Конфиденциальность", Risk = ServiceRisk.Safe },
        new() { ServiceName = "RetailDemo", DisplayName = "Демо-режим розницы", Description = "Демо-режим для магазинов.", Category = "Дополнительно", Risk = ServiceRisk.Safe },
        new() { ServiceName = "XblAuthManager", DisplayName = "Xbox Live: проверка подлинности", Description = "Аутентификация Xbox Live. Отключите, если не пользуетесь Xbox.", Category = "Игры", Risk = ServiceRisk.Caution },
        new() { ServiceName = "XblGameSave", DisplayName = "Xbox Live: сохранения", Description = "Синхронизация сохранений в облако.", Category = "Игры", Risk = ServiceRisk.Caution },
        new() { ServiceName = "XboxGipSvc", DisplayName = "Xbox: аксессуары", Description = "Управление аксессуарами Xbox.", Category = "Игры", Risk = ServiceRisk.Caution },
        new() { ServiceName = "XboxNetApiSvc", DisplayName = "Xbox Live: сеть", Description = "Сетевые функции Xbox Live.", Category = "Игры", Risk = ServiceRisk.Caution },
        new() { ServiceName = "bthserv", DisplayName = "Поддержка Bluetooth", Description = "Управление устройствами Bluetooth. Отключите, если не используете.", Category = "Оборудование", Risk = ServiceRisk.Caution },
        new() { ServiceName = "CDPUserSvc", DisplayName = "Платформа подключённых устройств", Description = "Функции работы с несколькими устройствами.", Category = "Дополнительно", Risk = ServiceRisk.Safe },
        new() { ServiceName = "PimIndexMaintenanceSvc", DisplayName = "Данные контактов", Description = "Индексация контактов для быстрого поиска.", Category = "Дополнительно", Risk = ServiceRisk.Safe },
        new() { ServiceName = "WMPNetworkSvc", DisplayName = "Сеть Windows Media Player", Description = "Общий доступ к библиотеке WMP.", Category = "Дополнительно", Risk = ServiceRisk.Safe },
        new() { ServiceName = "WerSvc", DisplayName = "Отчёт об ошибках Windows", Description = "Отправка отчётов о сбоях в Microsoft.", Category = "Конфиденциальность", Risk = ServiceRisk.Safe },
    ];
}
