using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SonicBoost.Core.Backup;
using SonicBoost.Core.Debloat;
using SonicBoost.Core.Drivers;
using SonicBoost.Core.Hardware;
using SonicBoost.Core.Network;
using SonicBoost.Core.Power;
using SonicBoost.Core.Privacy;
using SonicBoost.Core.Services;
using SonicBoost.Core.Tweaks;
using SonicBoost.ViewModels;
using SonicBoost.Views;
using System.Windows;

namespace SonicBoost;

public partial class App : Application
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
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

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        GetService<MainWindow>().Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}
