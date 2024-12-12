using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Win32;
using NMSE.RTemplates;
using NMSE.ViewModels;
using Serilog;
using Application = System.Windows.Application;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace NMSE.Views;

public partial class RunSimulationPage : INotifyPropertyChanged {
  #region DependencyProperty bool ShowROutput
  public static readonly DependencyProperty ShowROutputProperty = DependencyProperty.Register(nameof(ShowROutput), typeof(bool), typeof(RunSimulationPage),
    new FrameworkPropertyMetadata(default(bool)));
  public bool ShowROutput { get => (bool)GetValue(ShowROutputProperty); set => SetValue(ShowROutputProperty, value); }
  #endregion // DependencyProperty bool ShowROutput

  #region DependencyProperty bool RunSimulation
  public static readonly DependencyProperty RunSimulationProperty = DependencyProperty.Register(nameof(RunSimulation), typeof(bool), typeof(RunSimulationPage),
    new FrameworkPropertyMetadata(default(bool)));
  public bool RunSimulation { get => (bool)GetValue(RunSimulationProperty); set => SetValue(RunSimulationProperty, value); }
  #endregion // DependencyProperty bool RunSimulation

  #region DependencyProperty bool IsRuning
  public static readonly DependencyProperty IsRuningProperty = DependencyProperty.Register(nameof(IsRuning), typeof(bool), typeof(RunSimulationPage),
    new FrameworkPropertyMetadata(default(bool), IsRunningPropertyChanged));
  public bool IsRuning { get => (bool)GetValue(IsRuningProperty); set => SetValue(IsRuningProperty, value); }

  public bool IsNotRuning => !IsRuning;

  static void IsRunningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    ((RunSimulationPage)d).NotifyPropertyChanged(nameof(IsNotRuning));
  }

  #endregion // DependencyProperty bool IsRuning

  public RunSimulationPage(RunSimulationViewModel viewModel) {
    InitializeComponent();
    DataContext = viewModel;
    ShowROutput = true;
  }



  #region show messages to the user

  ListViewItem SpecialMessage(string MessageText, string MessageType) {
    if (MessageType == "success") {
      var MyMessage = new ListViewItem {
        Content = MessageText,
        Style = (Style)FindResource("ListViewItemSuccess")
      };
      return MyMessage;
    } else {
      var MyMessage = new ListViewItem {
        Content = MessageText,
        Style = (Style)FindResource("ListViewItemFailure")
      };
      return MyMessage;
    }
  }


  #endregion


  /// <summary>
  /// Ask the user for the parent folder to store the result folder.
  /// Then start running the simulation.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  async void Run_Button_Click(object sender, RoutedEventArgs e) {

    // Select the parent folder of the result folder
    var dlg = new OpenFolderDialog() { };
    if (dlg.ShowDialog(Application.Current.MainWindow) is not true)
      return;
    SimProb.Instance.ParentFolder = dlg.FolderName;


    try {
      PBar.Value = 0;
      PBar.IsEnabled = true;
      ProgressMessages.IsEnabled = true;

      Application.Current.Dispatcher.InvokeAsync(() => IsRuning = true, DispatcherPriority.Background);

      PBar.Value += 5;

      // Create folders to store the results
      try
        {
        RCodeGenerator.CreateFolderStructure();
        ProgressMessages.Items.Add($"Created folder structure at {SimProb.Instance.ResultFolder}");
        } catch (Exception) {
        Log.Error("Error while creating directory structure for results. Check file system permissions");
        throw;
      }
      PBar.Value += 5;

      try
        {
        RCodeGenerator.CopyPresetData();
        PBar.Value += 5;
        }
      catch (Exception)
        {
        SpecialMessage("Something went wrong while copying preset data!", "error");
        throw;
        }

      try
        {
        RCodeGenerator.GenerateReadMeFile();
        ProgressMessages.Items.Add("Created Readme file");
        }
      catch (Exception)
        {
        SpecialMessage("Something went wrong while creating the ReadMe-File!", "error");
        throw;
        }

      try
        {
        RCodeGenerator.CreateRCode();
        ProgressMessages.Items.Add("Created R-Code");
        PBar.Value += 15;
        }
      catch (Exception)
        {
        SpecialMessage("Something went wrong generating the R code!", "error");
        throw;
        }


      ProgressMessages.Items.Add("Run simulation");
      var scriptPath = Path.Combine(SimProb.Instance.ResultFolder, "simulation.R");
      var processInfo = new ProcessStartInfo {
        FileName = SimProb.Instance.PersistentData["SelectedVersionOfR"],
        Arguments = scriptPath,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true,
        WorkingDirectory = SimProb.Instance.ResultFolder
      };

      if (!RunSimulation)
        return;

      var command = $"\"{SimProb.Instance.PersistentData["SelectedVersionOfR"]}\" \"{scriptPath}\"";

      string errorText = null;
      using (var process = new Process()) {
        process.StartInfo = processInfo;
        process.Start();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(() => errorText = process.StandardError.ReadToEnd());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        //if (ShowROutput)
        for (; ; ) {
          var line = await process.StandardOutput.ReadLineAsync();
          if (line is null)
            break;
          if (ShowROutput)
            Application.Current.Dispatcher.InvokeAsync(() => ProgressMessages.Items.Add(line), DispatcherPriority.Background);
        }
        //        else {
        //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //          Task.Run(() => process.StandardOutput.ReadToEnd());
        //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //          await process.WaitForExitAsync();
        //        }
      }


      // gather preview images
        foreach (string file in Directory.GetFiles(SimProb.Instance.FiguresFolder, "*.png"))
        {
                SimProb.Instance.PreviewImages.Add(new ImageModel { ImagePath = file });
        }
     





      PBar.Value = 100;

      Application.Current.Dispatcher.InvokeAsync(() => {
        ProgressMessages.Items.Add(!string.IsNullOrWhiteSpace(errorText)
          ? SpecialMessage(errorText, "error")
          : SpecialMessage("Succesfully completed", "success"));
      }, DispatcherPriority.Background);

      SimProb.Instance.ChangeMenuItemByTag("SimulationResults", true, true);
    } catch (Exception exc) { } finally {
      Application.Current.Dispatcher.InvokeAsync(() => IsRuning = false, DispatcherPriority.Background);
    }
    
  }

  public event PropertyChangedEventHandler PropertyChanged;
  internal void NotifyPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new(propertyName)); }
}
