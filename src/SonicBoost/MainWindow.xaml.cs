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
    private readonly Frame _contentFrame;

    public MainWindow()
    {
        InitializeComponent();
        _contentFrame = new Frame { NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden };
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        NavigateTo(typeof(DashboardPage));
    }

    private void RootNavigation_SelectionChanged(NavigationView sender, RoutedEventArgs args)
    {
        if (sender.SelectedItem is NavigationViewItem item && item.TargetPageType is not null)
        {
            NavigateTo(item.TargetPageType);
        }
    }

    private void NavigateTo(Type pageType)
    {
        object? page = pageType.Name switch
        {
            nameof(DashboardPage) => new DashboardPage(App.GetService<DashboardViewModel>()),
            nameof(TweaksPage) => new TweaksPage(App.GetService<TweaksViewModel>()),
            nameof(ServicesPage) => new ServicesPage(App.GetService<ServicesViewModel>()),
            nameof(PrivacyPage) => new PrivacyPage(App.GetService<PrivacyViewModel>()),
            nameof(DebloatPage) => new DebloatPage(App.GetService<DebloatViewModel>()),
            nameof(NetworkPage) => new NetworkPage(App.GetService<NetworkViewModel>()),
            nameof(DriversPage) => new DriversPage(App.GetService<DriversViewModel>()),
            nameof(PowerPage) => new PowerPage(App.GetService<PowerViewModel>()),
            _ => null
        };

        if (page is Page p)
        {
            RootNavigation.NavigateWithHierarchy(pageType);
        }
    }

    private void SonicVpnCard_Click(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://sonicvpn.com") { UseShellExecute = true });
    }
}
