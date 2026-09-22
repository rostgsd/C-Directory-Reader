using System.Text.Json;
using System.Reflection;
using Microsoft.Extensions.Configuration;
namespace Directory_Reader;

// do not manually set
public class AppSettings { 
    public string StartColor { get; set; }
    public string EndColor { get; set; }
    public int SampleSize { get; set; }
    public string AnalysisMode { get; set; }
    public bool IgnoreHiddenContents { get; set; }
}
public static class GlobalConfig {
    public static AppSettings Settings { get; }
    private static PropertyInfo[] _properties = typeof(AppSettings).GetProperties();
    private static FieldInfo[] _colors = typeof(PresetColors).GetFields();

    static GlobalConfig() {
        string currDir = AppContext.BaseDirectory;
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(currDir)
            .AddJsonFile(Path.Combine(currDir, "AppSettings.json"), optional: false, reloadOnChange: true)
            .Build();
        
        Settings = new AppSettings();
        configuration.Bind(Settings);
    }

    public static void SaveConfigs() {
        string jasn = JsonSerializer.Serialize(Settings, new JsonSerializerOptions {WriteIndented = true});
        File.WriteAllText("appSettings.json", jasn);
    }
    
    // for use in parseCLArgs
    public static void ValidateNewConfig(string arg, string val) {
        bool propertyExists = _properties.Any(x =>
            string.Equals(x.Name, arg, StringComparison.OrdinalIgnoreCase));
        if (!propertyExists) {
            throw new ArgumentException($"Invalid configuration argument: '{arg}'");
        }

        if (string.IsNullOrWhiteSpace(val)) {
            throw new ArgumentException($"Unknown argument: '{arg}'");
        }
        
        bool isValid = arg.ToLowerInvariant() switch {
            "startcolor" or "endcolor" =>
                _colors.Any(c => string.Equals(c.Name, val, StringComparison.OrdinalIgnoreCase)),
            "samplesize" =>
                int.TryParse(val, out _),
            "analysismode" =>
                Enum.TryParse<AnalysisMode>(val, ignoreCase: true, out _),
            "ignorehiddencontent" =>
                bool.TryParse(val, out _),
            _ => false
        };
        if (!isValid) {
            throw new ArgumentException($"Invalid value '{val}' for argument '{arg}'");
        }
    }
    public static void ModifyConfig(string arg, string val) {
        PropertyInfo? property = _properties.FirstOrDefault(p => 
            string.Equals(p.Name, arg, StringComparison.OrdinalIgnoreCase));
        if (property == null) {
            throw new NullReferenceException("configuration could not be found");
        }
        if (!property.CanWrite) {
            throw new ArgumentException("This configuration is read only");
        }
        // convert val
        var convertedVal = Convert.ChangeType(val, property.PropertyType);
        property.SetValue(Settings, convertedVal);
    }
    // match color json with object
    public static RGB MatchColorWithRgbObject(string colorX) {
        var match = _colors.FirstOrDefault(c =>
            string.Equals(c.Name, colorX, StringComparison.OrdinalIgnoreCase));
        return match?.GetValue(null) is RGB rgb? rgb : PresetColors.White;
    }
}