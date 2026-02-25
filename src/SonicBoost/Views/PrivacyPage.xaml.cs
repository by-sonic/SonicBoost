using SonicBoost.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SonicBoost.Views;

public partial class PrivacyPage : Page
{
    private readonly PrivacyViewModel _vm;

    public PrivacyPage(PrivacyViewModel vm)
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
