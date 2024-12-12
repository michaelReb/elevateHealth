using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using NMSE.Properties;
using NMSE.ViewModels;
using Serilog;

namespace NMSE;

public sealed class SimProb : NotifyPropertyChangedBase {


  bool _simulationCanRun;
  public bool SimulationCanRun { get => _simulationCanRun; set => SetProperty(ref _simulationCanRun, value); }



  #region model selection

  // Properties for project information

  bool _AllowModelChange;
  public bool AllowModelChange { get => _AllowModelChange; set => SetProperty(ref _AllowModelChange, value); }


  SimulationModelViewModel _SelectedModel = SimulationModelViewModel.AvailableModels[0];
  public SimulationModelViewModel SelectedModel {
    get => _SelectedModel;
    set {
      SetProperty(ref _SelectedModel, value);
    }
  }

  #endregion

  #region descriptive properties set by the user

  string _projectTitle;

  public string ProjectTitle {
    get => _projectTitle;
    set {
      SetProperty(ref _projectTitle, value);
      CheckNextStep();
    }
  }

  string _projectAuthors;
  public string ProjectAuthors {
    get => _projectAuthors;
    set {
      SetProperty(ref _projectAuthors, value);
      CheckNextStep();
    }
  }

  string _projectDescription;
  public string ProjectDescription { get => _projectDescription; set => SetProperty(ref _projectDescription, value); }


  #endregion

  #region result storage settings
  private string _ParentFolder;

  public string ParentFolder { get => _ParentFolder; set => SetProperty(ref _ParentFolder, value); }

  private string _ResultFolder;

  public string ResultFolder { get => _ResultFolder; set => SetProperty(ref _ResultFolder, value); }


  private string _DataFolder;

  public string DataFolder { get => _DataFolder; set => SetProperty(ref _DataFolder, value); }

  private string _FiguresFolder;

  public string FiguresFolder { get => _FiguresFolder; set => SetProperty(ref _FiguresFolder, value); }

  #endregion

  #region settings for logging and persistent data

  string _sConfigFolderPath;
  public string sConfigFolderPath { get => _sConfigFolderPath; set => SetProperty(ref _sConfigFolderPath, value); }
  string _sConfigFilePath;
  public string sConfigFilePath { get => _sConfigFilePath; set => SetProperty(ref _sConfigFilePath, value); }

  string _sDebugLogFolder;
  public string sDebugLogFolder { get => _sDebugLogFolder; set => SetProperty(ref _sDebugLogFolder, value); }

  string _sDebugLogFilePath;
  public string sDebugLogFilePath { get => _sDebugLogFilePath; set => SetProperty(ref _sDebugLogFilePath, value); }

  public Dictionary<string, string> PersistentData = new Dictionary<string, string>();

  #endregion

  #region Result preview

  public ObservableCollection<ImageModel> PreviewImages {  get; set; }


  #endregion

  /// <summary>
  /// Called then either the title or the authors of the projects were set
  /// </summary>
  void CheckNextStep() {
    var title = ProjectTitle;
    var authors = ProjectAuthors;
    if (string.IsNullOrWhiteSpace(title)
        || title.Length < 5
        || string.IsNullOrWhiteSpace(authors)
        || authors.Length < 2) {
      return;}

    ChangeMenuItemByTag("SettingsInputRates", true, true);
    ChangeMenuItemByTag("SettingsInputWeeks", true, true);
    ChangeMenuItemByTag("SettingsInputInterventions", true, true);
    ChangeMenuItemByTag("RunSimulation", true, true);
  }

  /// <summary>
  ///  Private constructor to prevent external instantiation
  /// </summary>
  private SimProb() {
    InitDebugLog();

    SimulationCanRun = false;
    AllowModelChange = true;

    sConfigFolderPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      "NMSE");

    sConfigFilePath = Path.Combine(
      sConfigFolderPath,
      "NMSEAppProperties.json");

    PreviewImages = new ObservableCollection<ImageModel> { };

    LoadPersistentData();
  }



  #region Data Preset

  #region Data Preset: Patient Flow

  private double _ParamPrevalence;

  public double ParamPrevalence { get => _ParamPrevalence; set => SetProperty(ref _ParamPrevalence, value); }

  private double _ParamDiagnosisRate;

  public double ParamDiagnosisRate { get => _ParamDiagnosisRate; set => SetProperty(ref _ParamDiagnosisRate, value); }



  private double _ParamTreatmentRate;

  public double ParamTreatmentRate { get => _ParamTreatmentRate; set => SetProperty(ref _ParamTreatmentRate, value); }

  private double _ParamUncontrolledStandard;

  public double ParamUncontrolledStandard { get => _ParamUncontrolledStandard; set => SetProperty(ref _ParamUncontrolledStandard, value); }

  private double _ParamaControlledStandard;

  public double ParamaControlledStandard { get => _ParamaControlledStandard; set => SetProperty(ref _ParamaControlledStandard, value); }

  private double _ParamSymptomFreeStandard;

  public double ParamSymptomFreeStandard { get => _ParamSymptomFreeStandard; set => SetProperty(ref _ParamSymptomFreeStandard, value); }






  public Dictionary<string, Dictionary<string, double>> ModelParametersSourceRates = new Dictionary<string, Dictionary<string, double>>
        {
            { "prevalence", new Dictionary<string, double> {
                { "Base", 0.007 },
                { "BaseImproved", 0.007 },
                { "Bridge", 0.007 },
                { "DigiMoc", 0.007 } } },
            { "diagnosis_rate", new Dictionary<string, double> {
                { "Base", 0.81 },
                { "BaseImproved", 0.81 },
                { "Bridge", 0.81 },
                { "DigiMoc", 0.81 } } },
            { "treatment_rate", new Dictionary<string, double> {
                { "Base", 0.653 },
                { "BaseImproved", 0.653 },
                { "Bridge", 0.653 },
                { "DigiMoc", 0.653 } } },
            { "uncontrolled_standard", new Dictionary<string, double> {
                { "Base", 0.116 },
                { "BaseImproved", 0.116 },
                { "Bridge", 0.116 },
                { "DigiMoc", 0.116 } } },
            { "controlled_standard", new Dictionary<string, double> {
                { "Base", 0.060 },
                { "BaseImproved", 0.060 },
                { "Bridge", 0.060 },
                { "DigiMoc", 0.060 } } },
            { "symptom_free_standard", new Dictionary<string, double> {
                { "Base", 0.015 },
                { "BaseImproved", 0.015 },
                { "Bridge", 0.015 },
                { "DigiMoc", 0.015 } } }
        };

  public void SetDefaultsForRates()
    {
    ParamPrevalence = ModelParametersSourceRates["prevalence"][SimProb.Instance.SelectedModel.Name];
    ParamDiagnosisRate = ModelParametersSourceRates["diagnosis_rate"][SimProb.Instance.SelectedModel.Name];
    ParamTreatmentRate = ModelParametersSourceRates["treatment_rate"][SimProb.Instance.SelectedModel.Name];
    ParamUncontrolledStandard = ModelParametersSourceRates["uncontrolled_standard"][SimProb.Instance.SelectedModel.Name];
    ParamaControlledStandard = ModelParametersSourceRates["controlled_standard"][SimProb.Instance.SelectedModel.Name];
    ParamSymptomFreeStandard = ModelParametersSourceRates["symptom_free_standard"][SimProb.Instance.SelectedModel.Name];

    }


  #endregion


  #region Data Preset: Time Relations


  private int _ParamDiagnosisTime;

  public int ParamDiagnosisTime { get => _ParamDiagnosisTime; set => SetProperty(ref _ParamDiagnosisTime, value); }

  private int _ParamTreatmentInitiationTime;

  public int ParamTreatmentInitiationTime { get => _ParamTreatmentInitiationTime; set => SetProperty(ref _ParamTreatmentInitiationTime, value); }

  private int _ParamTreatmentEscalationTime1;

  public int ParamTreatmentEscalationTime1 { get => _ParamTreatmentEscalationTime1; set => SetProperty(ref _ParamTreatmentEscalationTime1, value); }

  private int _ParamTreatmentEscalationTime2;

  public int ParamTreatmentEscalationTime2 { get => _ParamTreatmentEscalationTime2; set => SetProperty(ref _ParamTreatmentEscalationTime2, value); }
  




  public Dictionary<string, Dictionary<string, int>> ModelParametersSourceWeeks = new Dictionary<string, Dictionary<string, int>>
        {
            { "diagnosis_time", new Dictionary<string, int> {
                { "Base", 104 },
                { "BaseImproved", 104 },
                { "Bridge", 104 },
                { "DigiMoc", 104 } } },
            { "treatment_initiation_time", new Dictionary<string, int> {
                { "Base", 13 },
                { "BaseImproved", 13 },
                { "Bridge", 13 },
                { "DigiMoc", 13 } } },
            { "treatment_escalation_time1", new Dictionary<string, int> {
                { "Base", 12 },
                { "BaseImproved", 12 },
                { "Bridge", 12 },
                { "DigiMoc", 12 } } },
            { "treatment_escalation_time2", new Dictionary<string, int> {
                { "Base", 12 },
                { "BaseImproved", 12 },
                { "Bridge", 12 },
                { "DigiMoc", 12 } } }
        };


  public void SetDefaultForWeeks()
    {
    ParamDiagnosisTime = ModelParametersSourceWeeks["diagnosis_time"][SimProb.Instance.SelectedModel.Name];
    ParamTreatmentInitiationTime = ModelParametersSourceWeeks["treatment_initiation_time"][SimProb.Instance.SelectedModel.Name];
    ParamTreatmentEscalationTime1 = ModelParametersSourceWeeks["treatment_escalation_time1"][SimProb.Instance.SelectedModel.Name];
    ParamTreatmentEscalationTime2 = ModelParametersSourceWeeks["treatment_escalation_time2"][SimProb.Instance.SelectedModel.Name];

    }

  #endregion


  #region Data Preset: Interventions

  private double _ParamAwarenessIntervention;

  public double ParamAwarenessIntervention { get => _ParamAwarenessIntervention; set => SetProperty(ref _ParamAwarenessIntervention, value); }


  private double _ParamEducationInterventionPatients;

  public double ParamEducationInterventionPatients { get => _ParamEducationInterventionPatients; set => SetProperty(ref _ParamEducationInterventionPatients, value); }


  private double _ParamEducationInterventionPhysiciansStart;

  public double ParamEducationInterventionPhysiciansStart { get => _ParamEducationInterventionPhysiciansStart; set => SetProperty(ref _ParamEducationInterventionPhysiciansStart, value); }


  private double _ParamEducationInterventionPhysiciansEscalation;

  public double ParamEducationInterventionPhysiciansEscalation { get => _ParamEducationInterventionPhysiciansEscalation; set => SetProperty(ref _ParamEducationInterventionPhysiciansEscalation, value); }


  public Dictionary<string, Dictionary<string, double>> ModelParametersSourceInterventions = new Dictionary<string, Dictionary<string, double>>
        {
            { "AwarenessIntervention", new Dictionary<string, double> {
                { "Base", 0.2 },
                { "BaseImproved", 0.2 },
                { "Bridge", 0.2 },
                { "DigiMoc", 0.2 } } },
            { "EducationInterventionPatients", new Dictionary<string, double> {
                { "Base", 0.2 },
                { "BaseImproved", 0.2 },
                { "Bridge", 0.2 },
                { "DigiMoc", 0.2 } } },
            { "EducationInterventionPhysisciansStart", new Dictionary<string, double> {
                { "Base", 0.14 },
                { "BaseImproved", 0.14 },
                { "Bridge", 0.14 },
                { "DigiMoc", 0.14 } } },
            { "EducationInterventionPhysisciansEscalation", new Dictionary<string, double> {
                { "Base", 0.22 },
                { "BaseImproved", 0.22 },
                { "Bridge", 0.22 },
                { "DigiMoc", 0.22 } } }
        };


  public void SetDefaultsForInterventions()
    {
    ParamAwarenessIntervention = ModelParametersSourceInterventions["AwarenessIntervention"][SimProb.Instance.SelectedModel.Name];
    ParamEducationInterventionPatients = ModelParametersSourceInterventions["EducationInterventionPatients"][SimProb.Instance.SelectedModel.Name];
    ParamEducationInterventionPhysiciansStart = ModelParametersSourceInterventions["EducationInterventionPhysisciansStart"][SimProb.Instance.SelectedModel.Name];
    ParamEducationInterventionPhysiciansEscalation = ModelParametersSourceInterventions["EducationInterventionPhysisciansEscalation"][SimProb.Instance.SelectedModel.Name];
    }

  #endregion


  #region Data Preset: Remission Probabilities


  private int _ParamRemissionProbT0;

  public int ParamRemissionProbT0 { get => _ParamRemissionProbT0; set => SetProperty(ref _ParamRemissionProbT0, value); }

  private int _ParamRemissionProbT1;

  public int ParamRemissionProbT1 { get => _ParamRemissionProbT1; set => SetProperty(ref _ParamRemissionProbT1, value); }

  private int _ParamRemissionProbT2;

  public int ParamRemissionProbT2 { get => _ParamRemissionProbT2; set => SetProperty(ref _ParamRemissionProbT2, value); }

  private int _ParamRemissionProbT3;

  public int ParamRemissionProbT3 { get => _ParamRemissionProbT3; set => SetProperty(ref _ParamRemissionProbT3, value); }

  private int _ParamRemissionProbT4;

  public int ParamRemissionProbT4 { get => _ParamRemissionProbT4; set => SetProperty(ref _ParamRemissionProbT4, value); }

  private int _ParamRemissionProbT5;

  public int ParamRemissionProbT5 { get => _ParamRemissionProbT5; set => SetProperty(ref _ParamRemissionProbT5, value); }


  public void SetDefaultForRemissionPropabilities()
    {
    ParamRemissionProbT0 = 1;
    ParamRemissionProbT1 = 52;
    ParamRemissionProbT2 = 103;
    ParamRemissionProbT3 = 156;
    ParamRemissionProbT4 = 261;
    ParamRemissionProbT5 = 520;

    }


  #endregion


  #endregion



  // Singleton instance
  private static readonly Lazy<SimProb> _instance = new Lazy<SimProb>(() => new SimProb());

  // Public property to access the singleton instance
  public static SimProb Instance => _instance.Value;


  public event PropertyChangedEventHandler? PropertyChanged;
  protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }


  private void InitDebugLog() {
    sDebugLogFolder = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
      "NMSEDebugLogs");
    if (!Directory.Exists(sDebugLogFolder)) {
      Directory.CreateDirectory(sDebugLogFolder);
    }

    sDebugLogFilePath = Path.Combine(sDebugLogFolder, "NMSE-DebugLog-.txt");
  }


  public void CheckIfFilesCanBeGenerated() {
    if (!String.IsNullOrEmpty(ProjectTitle) && !String.IsNullOrEmpty(ProjectAuthors) && !String.IsNullOrEmpty(ParentFolder)) {
      SimulationCanRun = true;
    } else {
      SimulationCanRun = false;
    }
  }


  #region Data Persistence

  /// <summary>
  ///   Write all key-value-pairs from the dictionary to the file system.
  /// </summary>
  public void StorePersistentData() {
    if (!Directory.Exists(sConfigFolderPath)) {
      Directory.CreateDirectory(sConfigFolderPath);
    }

    string fileContent = JsonConvert.SerializeObject(PersistentData, Formatting.Indented);
    File.WriteAllText(sConfigFilePath, fileContent, Encoding.UTF8);
  }


  /// <summary>
  ///   Try to load data from the local config file
  /// </summary>
  public void LoadPersistentData() {
    PersistentData.Clear(); // discard any values load before
    if (!File.Exists(sConfigFilePath)) {
      Log.Information($"No configuration file found");
      return;
    }

    Log.Debug("Found config file");
    var json = File.ReadAllText(sConfigFilePath);
    var properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    if (properties == null) {
      return;
    }

    foreach (var kvp in properties) {
      PersistentData.Add(kvp.Key, kvp.Value);
      int c = PersistentData.Count;
    }
  }


  /// <summary>
  ///   Add or update a key-value-pair in the dictionary and then persist the data
  /// </summary>
  /// <param name="KeyString">the string of the key name</param>
  /// <param name="ValueString">the string of the value</param>
  public void PersistKeyValuePair(string KeyString, string ValueString) {
    KeyString = KeyString.Trim();
    // either update or add to the persistent data dictionary
    if (PersistentData.ContainsKey(KeyString)) {
      PersistentData[KeyString.Trim()] = ValueString;
    } else {
      PersistentData.Add(KeyString, ValueString);
    }

    StorePersistentData();
  }

  #endregion



#region Hamburger Menu

// TODO: Change the icons and titles for all HamburgerMenuItems here.
public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>() {
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellSetupCheckPage,
      Glyph = "\uE721",
      TargetPageType = typeof(SetupCheckViewModel),
      Tag = "SetupCheck",
      IsVisible = true,
      IsEnabled = true
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellSelectModelPage,
      Glyph = "\uF180",
      TargetPageType = typeof(SelectModelViewModel),
      Tag = "SelectModel",
      IsVisible = true,
      IsEnabled = true
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellProjectDescriptionPage,
      Glyph = "\uE716",
      TargetPageType = typeof(ProjectDescriptionViewModel),
      Tag = "ProjectDescription",
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellData1Page,
      Glyph = "\uE762",
      TargetPageType = typeof(Data1ViewModel),
      Tag = "SettingsInputRates",
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellData2Page,
      Glyph = "\uE762",
      TargetPageType = typeof(Data2ViewModel),
      Tag = "SettingsInputWeeks",
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellData3Page,
      Glyph = "\uE762",
      TargetPageType = typeof(Data3ViewModel),
      Tag = "SettingsInputInterventions",
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellSimulationPropertiesPage,
      Glyph = "\uE771",
      TargetPageType = typeof(SimulationPropertiesViewModel),
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellInterventionsPage,
      Glyph = "\uE71C",
      TargetPageType = typeof(InterventionsViewModel),
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellOutputSettingsPage,
      Glyph = "\uE771",
      TargetPageType = typeof(OutputSettingsViewModel),
      Tag = "SettingsDigiMocModel",
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellRunSimulationPage,
      Glyph = "\uE768",
      TargetPageType = typeof(RunSimulationViewModel),
      Tag = "RunSimulation",
      IsVisible = false,
      IsEnabled = false
    },
    new HamburgerMenuGlyphItem() {
      Label = Resources.ShellSimulationResultsPage,
      Glyph = "\uE9F9",
      TargetPageType = typeof(SimulationResultsViewModel),
      Tag = "SimulationResults",
      IsVisible = false,
      IsEnabled = false
    }
  };


  public void ChangeMenuItemByTag(string tag, bool bVisible = false, bool bEnabled = false) {
    if (bEnabled && !bVisible) {
      throw new ArgumentException("Menu item cannot be enabled while not visible");
    }

    foreach (var item in MenuItems) {
      if ((string)item.Tag != tag)
        continue;
      item.IsVisible = bVisible;
      item.IsEnabled = bEnabled;
    }
  }


  public void MarkStateByTag(string tag, bool TaskIsDone) {
    foreach (var item in MenuItems) {
      if ((string)item.Tag == tag) {
        // TODO
      }
    }
  }

  #endregion




  }
