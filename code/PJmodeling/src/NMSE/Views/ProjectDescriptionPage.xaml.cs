using System.Windows.Controls;
using NMSE.ViewModels;
using Serilog;

namespace NMSE.Views;

public partial class ProjectDescriptionPage : Page {
  public ProjectDescriptionPage(ProjectDescriptionViewModel viewModel) {
    InitializeComponent();
    DataContext = SimProb.Instance;
  }
}
