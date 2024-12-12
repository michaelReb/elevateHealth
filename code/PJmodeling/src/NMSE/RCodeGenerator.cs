using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMSE.RTemplates;

namespace NMSE;

public class RCodeGenerator
  {



  /// <summary>
  /// Create the folder structure to store the generated R code and the results from running it.
  /// The user selected a parent folder via the GUI. The created folder's name contains a timestamp
  /// in order to avaoid overriding previous results.
  /// </summary>
  public static void CreateFolderStructure()
    {
    // Define folder paths
    string[] PathComponents = {
         };
    SimProb.Instance.ResultFolder = Path.Combine(
      SimProb.Instance.ParentFolder,
      $"{DateTime.Now.ToString("yyyy-MM-dd_HHmmss-").ToString(CultureInfo.InvariantCulture)}Simulation");
    // folders below that
    SimProb.Instance.DataFolder = Path.Combine(SimProb.Instance.ResultFolder, "data");
    SimProb.Instance.FiguresFolder = Path.Combine(SimProb.Instance.ResultFolder, "figures");

    // Create the folders
    Directory.CreateDirectory(SimProb.Instance.ResultFolder);
    Directory.CreateDirectory(SimProb.Instance.DataFolder);
    Directory.CreateDirectory(SimProb.Instance.ResultFolder);

    }

  /// <summary>
  /// While the application allows the user to change settings, all the models still need
  /// some preset precalculated data. These are stored in rds-files (R's native format) and
  /// are loaded by the R-code. This function copies the data files need for the specific model
  /// into the data folder.
  /// Additionaly it copies functions.R which contains R methods used by multiple models.
  /// </summary>
  public static void CopyPresetData()
    {
    // all models share the same R-file with helper functions.
    File.Copy(@".\data\R\functions.R",
      Path.Combine(SimProb.Instance.ResultFolder, "functions.R"));
    
    // preset data are model specific 
    switch (SimProb.Instance.SelectedModel.Name)
      {
      case "Base" or "BaseImproved":
        File.Copy(
          @".\data\BaseModelImproved\df_rates.rds",
          Path.Combine(SimProb.Instance.DataFolder, "df_rates.rds"));
        File.Copy(
          @".\data\BaseModelImproved\df_remission.rds",
          Path.Combine(SimProb.Instance.DataFolder, "df_remission.rds"));
        File.Copy(
          @".\data\BaseModelImproved\df_UCT.rds",
          Path.Combine(SimProb.Instance.DataFolder, "df_UCT.rds"));
        break;
      case "Bridge":
        break;
      case "DigiMoc":
        break;
      }
   }




  /// <summary>
  /// Generates a README file that contains information about the simulation generated
  /// </summary>
  public async static void GenerateReadMeFile()
    {
    ReadMeFileTemplate readMeFile = new ReadMeFileTemplate();
    var ReadMeContent = readMeFile.TransformText();
    var ReadMeFilePath = Path.Combine(SimProb.Instance.ResultFolder, "ReadMe.txt");
    await File.WriteAllTextAsync(ReadMeFilePath, ReadMeContent);
    }



/// <summary>
/// This creates the R files based on the right template for the selected model.
/// </summary>
public static async void CreateRCode()
  {

    var scriptPath = Path.Combine(SimProb.Instance.ResultFolder, "simulation.R");

    switch (SimProb.Instance.SelectedModel.Name)
      {
      case "Base":
        BaseModel basemodelfile = new();
        var BaseModelMain = basemodelfile.TransformText();
        await File.WriteAllTextAsync(scriptPath, BaseModelMain);
        break;
      case "BaseImproved":
        BaseModelmproved maincodefile = new();
        var MainRContent = maincodefile.TransformText();
        await File.WriteAllTextAsync(scriptPath, MainRContent);
        break;
      case "Bridge":
        BridgeModel bridgemodelfile = new();
        var BridgeModelMain = bridgemodelfile.TransformText();
        await File.WriteAllTextAsync(scriptPath, BridgeModelMain);
        break;
      case "DigiMoc":
        DigiMocModel digimocmodelfile = new();
        var DigiMocMain = digimocmodelfile.TransformText();
        await File.WriteAllTextAsync(scriptPath, DigiMocMain);
        break;
      }



    }


}