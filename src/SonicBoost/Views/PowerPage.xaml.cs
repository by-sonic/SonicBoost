using SonicBoost.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SonicBoost.Views;

public partial class PowerPage : Page
{
    private readonly PowerViewModel _vm;

    public PowerPage(PowerViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vm.Plans.Count == 0)
            await _vm.LoadPlansCommand.ExecuteAsync(null);
    }
}
