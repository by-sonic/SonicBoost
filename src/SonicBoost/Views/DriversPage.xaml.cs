using SonicBoost.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SonicBoost.Views;

public partial class DriversPage : Page
{
    private readonly DriversViewModel _vm;

    public DriversPage(DriversViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vm.Recommendations.Count == 0)
            await _vm.LoadDriversCommand.ExecuteAsync(null);
    }
}
