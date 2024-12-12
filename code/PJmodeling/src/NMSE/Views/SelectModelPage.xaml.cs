using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NMSE.ViewModels;
using Image = System.Windows.Controls.Image;

using Serilog;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using MahApps.Metro.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms;

namespace NMSE.Views;

public partial class SelectModelPage : Page, INotifyPropertyChanged
  {


  public SelectModelPage(SelectModelViewModel viewModel)
    {
        InitializeComponent();
        DataContext = SimProb.Instance;
        //SimProb.Instance.SelectedModel = "Base Model";
    }


  public event PropertyChangedEventHandler PropertyChanged;

  private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
    if (Equals(storage, value))
      {
      return;
      }

    storage = value;
    OnPropertyChanged(propertyName);
    }

  private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


  private void BaseModelExample_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    //// Create a new window to display the enlarged image
    var enlargedImageWindow = new Window
      {
      Title = "Enlarged Image",
      Width = 800,
      Height = 600,
      Content = new Image
        {
        //Source = new BitmapImage(new Uri("pack://application:,,,/NMSE;component/Images/transitions-base-model.png")),
        Source = new BitmapImage(new Uri("pack://application:,,,/NMSE;Images/transitions-base-model.png")),
        //Source = new BitmapImage(new Uri("/Images/transitions-base-model.png", UriKind.Relative)),
        Stretch = System.Windows.Media.Stretch.Uniform
        }
      };

    //var enlargeMe = new EnlargeAbleImage();
    //var MyWindow = enlargeMe.ReturnEnlargedWindow;

    // Show the window
    enlargedImageWindow.ShowDialog();
    }


  private void ModelImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
    //              Source="/Images/transitions-base-model.png"

    }

  private void SelectModel_Click(object sender, RoutedEventArgs e)
    {
    // deactivate button
    SimProb.Instance.AllowModelChange = false;

    Log.Debug($"{SimProb.Instance.SelectedModel.Name}");

    // As the model is selected, preset values for GUI elements on pages about to be shown
    SimProb.Instance.SetDefaultsForRates();
    SimProb.Instance.SetDefaultForWeeks();
    SimProb.Instance.SetDefaultsForInterventions();
    SimProb.Instance.SetDefaultForRemissionPropabilities();


    // CHANGE NAVIGATION
    // This would be the opportunity to introduce pages that are only available if a specific
    // model was selected
    SimProb.Instance.ChangeMenuItemByTag("ProjectDescription", true, true);
    SimProb.Instance.ChangeMenuItemByTag("SettingsInputRates", true, false);
    SimProb.Instance.ChangeMenuItemByTag("SettingsInputWeeks", true, false);
    SimProb.Instance.ChangeMenuItemByTag("SettingsInputInterventions", true, false);
    SimProb.Instance.ChangeMenuItemByTag("RunSimulation", true, false);
    SimProb.Instance.ChangeMenuItemByTag("SimulationResults", true, false);
    }
  }
