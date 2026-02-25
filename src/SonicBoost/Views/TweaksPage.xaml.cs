using SonicBoost.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SonicBoost.Views;

public partial class TweaksPage : Page
{
    private readonly TweaksViewModel _vm;

    public TweaksPage(TweaksViewModel vm)
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
}
