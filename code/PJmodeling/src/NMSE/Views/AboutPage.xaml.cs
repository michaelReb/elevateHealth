using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;

using NMSE.ViewModels;

namespace NMSE.Views;

public partial class AboutPage : Page
{
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

  /// <summary>
  /// Handels the click on a hyperlink and opens the URI in the default browser.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
    using (Process p = new Process())
      {
      p.StartInfo.FileName = e.Uri.AbsoluteUri;
      p.StartInfo.UseShellExecute = true;
      p.Start();
      }
     e.Handled = true;
    }


  }
