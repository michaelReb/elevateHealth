using System.Windows;
using System.Windows.Controls;
using NMSE.ViewModels;
using Serilog;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;

namespace NMSE.Views;

public partial class SetupCheckPage : Page {
  readonly SimProb AppSetup = SimProb.Instance;

  public HealthCheckR HealthCheck = new();

  public SetupCheckPage(SetupCheckViewModel viewModel) {
    InitializeComponent();
    DataContext = viewModel;
    SetRStatus();
    RLibrariesOverview.ItemsSource = HealthCheck.RLibrariesStatus;
    string alreadyselected;
    AppSetup.PersistentData.TryGetValue("SelectedVersionOfR", out alreadyselected);
    if (!string.IsNullOrEmpty(alreadyselected) && HealthCheck.InstalledVersionsOfR.Contains(alreadyselected)) RVersionSelector.SelectedItem = alreadyselected;
    RLibrariesOverview.Items.Refresh();
  }


  public void SetRStatus() {
    // The selected version of R is available
    if (HealthCheck.SelectedRVersionIsAvailable) {
      RStatusDescription.Text = "Your R installation seems fine.";
      RStatusDescription.Style = (Style)Application.Current.Resources["MessageSuccess"];
    }
    // There is no version of R selected but there a multiple available
    // (if there was only available that was automatically set)
    else if (!HealthCheck.RIsSelected && HealthCheck.RIsVailable && HealthCheck.InstalledVersionsOfR.Count > 1) {
      RStatusDescription.Text = "There seem to be multiple versions of R on your system. Please select one. If the selected one causes issues switch to another one and please send us a bug report.";
      RStatusDescription.Style = (Style)Application.Current.Resources["MessageWarning"];
    } else if (!HealthCheck.RIsVailable) {
      RStatusDescription.Text = "We did not find any version of R on your system. Please install a current version to enable running the simulation. Until then you can generate the code, but cannot calculate results.";
      RStatusDescription.Style = (Style)Application.Current.Resources["MessageError"];
    }

    RVersionSelector.ItemsSource = HealthCheck.InstalledVersionsOfR;

    if (HealthCheck.SelectedRVersionIsAvailable && HealthCheck.AllRLibrariesAvailable) {
      RLibrariesStatusDesciption.Text = "All necessary R-libraries are installed";
      RLibrariesStatusDesciption.Style = (Style)Application.Current.Resources["MessageSuccess"];
    } else if (HealthCheck.SelectedRVersionIsAvailable && !HealthCheck.AllRLibrariesAvailable) {
      RLibrariesStatusDesciption.Text = "You are missing required R libraries. The simulation cannot run without them. Please install them and recheck.";
      RLibrariesStatusDesciption.Style = (Style)Application.Current.Resources["MessageError"];
    }
  }

  void RVersionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    var comboBox = sender as ComboBox;
    var selectedVersionPath = (string)comboBox.SelectedValue;
    Log.Information($"Selected R version in Dropdown: {selectedVersionPath}");
    AppSetup.PersistKeyValuePair("SelectedVersionOfR", selectedVersionPath);
    Log.Debug($"Changed selection for R-Version to: {selectedVersionPath}");
    SetRStatus();
  }
}
