using System.Windows.Controls;

using NMSE.ViewModels;

namespace NMSE.Views;

public partial class UserManualPage : Page
{
    public UserManualPage(UserManualViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
