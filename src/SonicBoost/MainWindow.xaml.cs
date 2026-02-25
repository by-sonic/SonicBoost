using SonicBoost.ViewModels;
using SonicBoost.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace SonicBoost;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        NavList.SelectedIndex = 0;
    }

    private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavList.SelectedItem is ListBoxItem item && item.Tag is string tag)
        {
            var page = CreatePage(tag);
            if (page != null)
                PageFrame.Navigate(page);
        }
    }

    private static object? CreatePage(string tag) => tag switch
    {
        "Dashboard" => new DashboardPage(App.GetService<DashboardViewModel>()),
        "Tweaks" => new TweaksPage(App.GetService<TweaksViewModel>()),
        "Services" => new ServicesPage(App.GetService<ServicesViewModel>()),
        "Privacy" => new PrivacyPage(App.GetService<PrivacyViewModel>()),
        "Debloat" => new DebloatPage(App.GetService<DebloatViewModel>()),
        "Network" => new NetworkPage(App.GetService<NetworkViewModel>()),
        "Drivers" => new DriversPage(App.GetService<DriversViewModel>()),
        "Power" => new PowerPage(App.GetService<PowerViewModel>()),
        _ => null
    };

    private void SonicVpnCard_Click(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://t.me/bysonicvpn_bot") { UseShellExecute = true });
    }
}
