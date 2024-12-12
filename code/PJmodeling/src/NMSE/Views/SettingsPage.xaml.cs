using System.Windows.Controls;

using NMSE.ViewModels;

namespace NMSE.Views;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
