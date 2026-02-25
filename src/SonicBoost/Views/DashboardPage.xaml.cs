using SonicBoost.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SonicBoost.Views;

public partial class DashboardPage : Page
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await _vm.LoadSystemInfoCommand.ExecuteAsync(null);
    }

    private void SonicVpnBanner_Click(object sender, MouseButtonEventArgs e)
    {
        OpenSonicVpn();
    }

    private void SonicVpnButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSonicVpn();
    }

    private static void OpenSonicVpn()
    {
        Process.Start(new ProcessStartInfo("https://sonicvpn.com") { UseShellExecute = true });
    }
}
