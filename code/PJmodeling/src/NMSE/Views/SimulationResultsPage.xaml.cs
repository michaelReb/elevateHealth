using System.Diagnostics;
using System.Windows.Controls;

using NMSE.ViewModels;

namespace NMSE.Views;

public partial class SimulationResultsPage : Page
{
    public SimulationResultsPage(SimulationResultsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = SimProb.Instance;
    }

  private void BtnOpenResultFolder_Click(object sender, System.Windows.RoutedEventArgs e)
    {
    Process.Start(new ProcessStartInfo()
      {
      FileName = SimProb.Instance.ResultFolder,
      UseShellExecute = true,
      Verb = "open"
      });
    
        }
    }
