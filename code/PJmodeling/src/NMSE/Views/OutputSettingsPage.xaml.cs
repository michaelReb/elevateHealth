using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

using Serilog;

using NMSE.ViewModels;

namespace NMSE.Views;



public partial class OutputSettingsPage : Page
{



  public string sOutputFolder {  get; set; }

    public OutputSettingsPage(OutputSettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = this; // viewModel;
    }

  private void SelectFolder(object sender, RoutedEventArgs e)
  {

    // dafür muss Windows Forms aktiviert werden, siehe:
    // https://www.youtube.com/watch?v=Heq8qve1Vts
    // Alias WinForms um Ambiguität mit WPF zu vermeiden
    WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();
    WinForms.DialogResult result = dialog.ShowDialog();

    if (result == WinForms.DialogResult.OK) // WinForms: OK statt true!
    {
      SimProb.Instance.ParentFolder = dialog.SelectedPath;
      SimProb.Instance.CheckIfFilesCanBeGenerated();
      }
    }

}
