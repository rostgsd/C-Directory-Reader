namespace Directory_Reader;

public class Program {
    public static List<string> PathNames = [];
    public static List<int> IndentVals = [];
    public static List<int> ColorVals = [];
    private static int _recursionDepth = -1;
    
    static void ParseCLArgs() {
        // does not support flags with no values
        var args = Environment.GetCommandLineArgs();
        if (args.Length == 1) {
            return;
        }

        for (int i = 1; i < args.Length; i++) {
            string arg;
            if (!args[i].StartsWith('-')) {
                continue;
            }
            arg = args[i].TrimStart('-');
            
            string value;
            if (i + 1 < args.Length && !args[i + 1].StartsWith('-')) {
                value = args[i + 1];
                i++;
            }
            else {
                value = "";
            }
            GlobalConfig.ValidateNewConfig(arg, value);
            GlobalConfig.ModifyConfig(arg, value);
        }
        GlobalConfig.SaveConfigs();
    }
    // utility
    static void SetForegroundColor(RGB color) {
        Console.Write("\u001b[38;2;{0};{1};{2};48;2;0;0;0m", color.R, color.G, color.B);
    }
    static void ResetForegroundColor() {
        Console.Write("\u001b[0m");
    }
    static void IndentOnRecursion(int depth) {
        for (int i = 0; i < depth; i++) {
            Console.Write("   ");
        }
    }
    static void ProcessDirectory(DirectoryInfo directoryInfo, Action<FileInfo> fileEvaluator ) {
        _recursionDepth++;
        // process the current directory
        PathNames.Add($"--{directoryInfo.Name}--");
        ColorVals.Add(-1);
        IndentVals.Add(_recursionDepth - 1);
        // process files
        var files = directoryInfo.GetFiles();
        if (GlobalConfig.Settings.IgnoreHiddenContents) {
            files = files.Where(f => !f.Name.StartsWith('.')).ToArray();
        }
        foreach (var file in files) {
            fileEvaluator(file);
            IndentVals.Add(_recursionDepth);
        }
        foreach (var subDirectoryInfo in directoryInfo.GetDirectories()) {
            if (subDirectoryInfo.Name.StartsWith('.') && GlobalConfig.Settings.IgnoreHiddenContents) {
                continue;
            }
            ProcessDirectory(subDirectoryInfo, fileEvaluator);
        }
        _recursionDepth--;
    }
    
    static void Main() {
        // process command line arguments
        ParseCLArgs();
        // generate colors
        var startColor = GlobalConfig.MatchColorWithRgbObject(GlobalConfig.Settings.StartColor);
        var endColor = GlobalConfig.MatchColorWithRgbObject(GlobalConfig.Settings.EndColor);
        ColorGradient colorManager = new(startColor, endColor, GlobalConfig.Settings.SampleSize);
        colorManager.GenerateGradient();
        // generate function
        if (!Enum.TryParse(GlobalConfig.Settings.AnalysisMode, true, out AnalysisMode mode)) {
            mode = AnalysisMode.None;
            GlobalConfig.SaveConfigs();
        }
        Console.WriteLine($"Analysis mode: {mode}");
        var evaluateLineItem = RunMode.GetEvaluator(mode);
        // process directory
        ProcessDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()), evaluateLineItem);
        if (PathNames.Count != IndentVals.Count || PathNames.Count != ColorVals.Count) {
            Console.Error.WriteLine("Failed to parse input directory");
            Environment.Exit(1);
        }
        // output colors
        var maxColorVal = ColorVals.Max();
        for (int i = 0; i < ColorVals.Count; i++) {
            IndentOnRecursion(IndentVals[i]);
            if (ColorVals[i] >= 0) {
                SetForegroundColor(colorManager.RgbFromRawInt(ColorVals[i],maxColorVal));
            } else {
                SetForegroundColor(PresetColors.White);
            }
            Console.Write(PathNames[i]);
            ResetForegroundColor();
            Console.WriteLine();
        }
    }
}