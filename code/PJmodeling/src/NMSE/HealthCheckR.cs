using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace NMSE;

public class HealthCheckR
  {

  public bool RIsVailable { get; set; }
  public bool RIsSelected { get; set; }
  public bool SelectedRVersionIsAvailable { get; set; }

  public List<string> NecessaryRLibraries = new List<string> { "ggplot2", "matrixdist", "tidyverse", "expm", "trustOptim", "pracma" }; // for test add "CertainlyNotInstalled"

  public bool AllRLibrariesAvailable { get; set; }

  public ObservableCollection<string> InstalledVersionsOfR { get; } = new ObservableCollection<string>();

  public ObservableCollection<string> InstalledRLibraries { get; } = new ObservableCollection<string>();

  public ObservableCollection<RLibrary> RLibrariesStatus { get; } = new ObservableCollection<RLibrary>();

  // There is no version of R selected but there is exactly one vailable

  public HealthCheckR()
    {

    var AppSetup = SimProb.Instance;

    // Gather all installed versions of R
    GetInstalledVersionsOfR();
    RIsVailable = InstalledVersionsOfR.Count > 0 ? true : false;

    // If a version of R is already selected, check if this one is in the list of those found
    string alreadyselected;
    AppSetup.PersistentData.TryGetValue("SelectedVersionOfR",out alreadyselected);
    if (!String.IsNullOrEmpty(alreadyselected) && InstalledVersionsOfR.Any(item => item == alreadyselected))
      {
      SelectedRVersionIsAvailable = true;
      }
    else
      {
      SelectedRVersionIsAvailable = false;
      }

    AllRLibrariesAvailable = false;
    if (RIsVailable)
      {
      // gather all installed libraries
      var InstalledRLibraries = GetRLibraries();
      // check which of the necessary libraries are installed
      int cMissingLibraries = 0;
      if (InstalledRLibraries.Any())
        {
        foreach (var NecessaryLib in NecessaryRLibraries)
          {
          if (InstalledRLibraries.Contains(NecessaryLib))
            {
            RLibrariesStatus.Add(new RLibrary(NecessaryLib, true, true));
            }
          else
            {
            RLibrariesStatus.Add(new RLibrary(NecessaryLib, true, false));
            cMissingLibraries++;
            }
          }
        }
      else
        {
        //Log.Debug("No R libraries installed");
        }

          AllRLibrariesAvailable = (cMissingLibraries ==  0) ? true : false;
          }


        }

      /// <summary>
      /// The R installation is considered healthy if:
      /// 1. R is installed
      /// 2. An existing version of R was selected by the user or there is only one
      /// 3. All necessary libraries are installed
      /// </summary>
      /// <returns></returns>
      public bool IsRInstallationHealthy()
        {
        // 
        return true;
        }


      public void GetInstalledVersionsOfR()
        {
        // Get the Program Files folder path
        string sProgramFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        // Search for all files named "r.exe"
        string sSearchPattern = "RScript.exe";

        var RExecutables = Directory.EnumerateFiles(sProgramFilesFolder, sSearchPattern, new EnumerationOptions
          {
          IgnoreInaccessible = true,
          RecurseSubdirectories = true,
          MaxRecursionDepth = 20

          });

        if (RExecutables.Any())
          {
          foreach (var item in RExecutables)
            {
            InstalledVersionsOfR.Add(item);
            }

          }
        }



  /// <summary>
  /// Check the file system for all versions of R
  /// </summary>
  /// <returns></returns>
      public List<string> GetRLibraries()
        {
        string sRLibrariesFolders = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
          "R",
          "win-library");
        IEnumerable<string> RLibraries = Directory.EnumerateDirectories(sRLibrariesFolders, "*", new EnumerationOptions
          {
          IgnoreInaccessible = true,
          RecurseSubdirectories = true,
          MaxRecursionDepth = 1
          });

        List<string> LibraryNames = new List<string>();
        foreach (var item in RLibraries)
          {
          var foldername = new DirectoryInfo(item).Name;
          LibraryNames.Add(foldername);
          }

          return LibraryNames;
        }







      }
    
