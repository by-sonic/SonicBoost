using SonicBoost.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SonicBoost.Views;

public partial class NetworkPage : Page
{
    private readonly NetworkViewModel _vm;

    public NetworkPage(NetworkViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vm.Tweaks.Count == 0)
            await _vm.LoadTweaksCommand.ExecuteAsync(null);
    }

    private void SonicVpnBlock_Click(object sender, MouseButtonEventArgs e) => OpenSonicVpn();
    private void SonicVpnButton_Click(object sender, RoutedEventArgs e) => OpenSonicVpn();

    private static void OpenSonicVpn()
    {
        Process.Start(new ProcessStartInfo("https://sonicvpn.com") { UseShellExecute = true });
    }
}
