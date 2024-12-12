using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSE;

  public class RLibrary
  {
  public bool LibraryIsNecessary { get; set; }
  public bool LibraryIsInstalled { get; set; }
  public string LibraryName { get; set; }

  public string? LibraryMinimumVersion { get; set; }

  public string? LibraryDescription { get; set; }

  public string LibraryStateSymbol { get; set; }

  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="name">The name of the library</param>
  /// <param name="isNecessary">Is this library required to run code?</param>
  /// <param name="isInstalled">Is this library installed?</param>
  /// <param name="description">Optional description of the library</param>
  public RLibrary(string name, bool isNecessary, bool isInstalled, string description = null)
      {
    LibraryName = name;
    LibraryIsNecessary = isNecessary;
    LibraryIsInstalled = isInstalled;
    LibraryDescription = description;
    LibraryStateSymbol = GetSymbolForState(isInstalled);
      }


  public string GetSymbolForState(bool BoolToGetTheSymbolFor)
    {
    string symbol = BoolToGetTheSymbolFor ? "✔️" : "⚠";
    return symbol;
    }
    

    }
