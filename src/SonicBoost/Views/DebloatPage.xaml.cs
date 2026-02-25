using SonicBoost.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SonicBoost.Views;

public partial class DebloatPage : Page
{
    private readonly DebloatViewModel _vm;

    public DebloatPage(DebloatViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vm.Apps.Count == 0)
            await _vm.LoadAppsCommand.ExecuteAsync(null);
    }
}
