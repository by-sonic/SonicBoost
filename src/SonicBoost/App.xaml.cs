using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Debloat;
using SonicBoost.Core.Drivers;
using SonicBoost.Core.Hardware;
using SonicBoost.Core.Logging;
using SonicBoost.Core.Network;
using SonicBoost.Core.Power;
using SonicBoost.Core.Privacy;
using SonicBoost.Core.Services;
using SonicBoost.Core.Tweaks;
using SonicBoost.ViewModels;
using SonicBoost.Views;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace SonicBoost;

public partial class App : Application
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<LogService>();
            services.AddSingleton<BackupService>();
            services.AddSingleton<HardwareDetectionService>();
            services.AddSingleton<TweakEngine>();
            services.AddSingleton<ServiceManager>();
            services.AddSingleton<PrivacyService>();
            services.AddSingleton<DebloatService>();
            services.AddSingleton<NetworkOptimizer>();
            services.AddSingleton<DriverService>();
            services.AddSingleton<PowerPlanService>();

            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<TweaksViewModel>();
            services.AddSingleton<ServicesViewModel>();
            services.AddSingleton<PrivacyViewModel>();
            services.AddSingleton<DebloatViewModel>();
            services.AddSingleton<NetworkViewModel>();
            services.AddSingleton<DriversViewModel>();
            services.AddSingleton<PowerViewModel>();

            services.AddSingleton<MainWindow>();
        })
        .Build();

    public static T GetService<T>() where T : class
        => _host.Services.GetRequiredService<T>();

    public static bool IsAdmin { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        IsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

        if (!IsAdmin)
        {
            var result = MessageBox.Show(
                "SonicBoost запущен без прав администратора.\n\n" +
                "Большинство оптимизаций требуют прав администратора для записи в реестр и управления службами.\n\n" +
                "Перезапустить от имени администратора?",
                "SonicBoost — требуются права",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName ?? "SonicBoost.exe",
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process.Start(psi);
                }
                catch
                {
                    // UAC denied
                }
                Shutdown();
                return;
            }
        }

        await _host.StartAsync();

        var log = GetService<LogService>();
        log.Info($"SonicBoost запущен. Админ: {IsAdmin}");

        GetService<MainWindow>().Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var log = GetService<LogService>();
        log.Info("SonicBoost завершён");

        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}
