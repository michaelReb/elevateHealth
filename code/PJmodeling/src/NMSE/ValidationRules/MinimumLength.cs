using System.Globalization;
using System.Windows.Controls;
using Serilog;

namespace NMSE.ValidationRules;

public class MinimumLength : ValidationRule {
  public int MinimumLengthChars { get; set; }

  public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
    var str = value as string;

    if (string.IsNullOrWhiteSpace(str)) {
      Log.Debug("Validation failed: string is empty");
      return new(false, "Please provide a value for this field.");
    }

    if (str.Length >= MinimumLengthChars) {
      Log.Debug("String has minimum length");
      return ValidationResult.ValidResult;
    }

    Log.Debug("Validation failed: string is not empty, but below minimum length");
    return new(false, $"Minimum length {MinimumLengthChars} characters");
  }
}
