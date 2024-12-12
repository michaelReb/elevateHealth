using System.Windows.Controls;

using NMSE.ViewModels;

namespace NMSE.Views;

public partial class SimulationPropertiesPage : Page
{
    public SimulationPropertiesPage(SimulationPropertiesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
