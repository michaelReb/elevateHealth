using System.Windows.Controls;

using NMSE.ViewModels;

namespace NMSE.Views;

public partial class InterventionsPage : Page
{
    public InterventionsPage(InterventionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
