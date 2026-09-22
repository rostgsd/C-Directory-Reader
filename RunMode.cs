namespace Directory_Reader;
/* to be used in recursion loop to generate table values for
 further calculation and displaying on screen */
public enum AnalysisMode {
    None,
    FileSize,
    Date,
    Extension,
    OrdinalString,
}
public static class RunMode {
    private static Dictionary<string, int> _extensionMappings;
    private static int[] _shuffledNumbers;
    private static int _index = 0;

    public static Action<FileInfo> GetEvaluator(AnalysisMode mode) {
        return mode switch {
            AnalysisMode.FileSize => file => EvaluateLineItem(file.Name, file.Length),
            AnalysisMode.Date => file => EvaluateLineItem(file.Name, file.CreationTime),
            AnalysisMode.Extension => file => EvaluateLineItem(file.Name, file.Extension),
            AnalysisMode.OrdinalString => file => EvaluateLineItem(file.Name),
            _ => file => {
                Program.PathNames.Add(file.Name);
                Program.ColorVals.Add(0);
            }
        };
    } 

    static RunMode() {
        _extensionMappings = new Dictionary<string, int>();
        _shuffledNumbers = Enumerable.Range(0, 1023).ToArray();
        Random.Shared.Shuffle(_shuffledNumbers);
    }

    private static void EvaluateLineItem(string name, long size) {
        double order = Math.Floor(Math.Log(size, 1000));
        string sizeLabel;
        Console.WriteLine(size);
        if (size <= 0) {
            order = 0;
        }

        var displaySize = size / Math.Pow(10, order*3);
        displaySize = Math.Round(displaySize, 1);
        switch (order) {
            case 1: sizeLabel = $" {displaySize}kB"; break;
            case 2: sizeLabel = $" {displaySize}MB"; break;
            case 3: sizeLabel = $" {displaySize}GB"; break;
            case 4: sizeLabel = $" {displaySize}TB"; break;
            default: sizeLabel = $" {size} bytes"; break;
        }
        Program.PathNames.Add(name + sizeLabel);
        Program.ColorVals.Add((int)size / 1000);
    }
    private static void EvaluateLineItem(string name, DateTime date) {
        Program.PathNames.Add(name + date.ToString("yyyy-MM-dd"));
        Program.ColorVals.Add((DateTime.Today - date.Date).Days);
    }
    private static void EvaluateLineItem(string name, string extension) {
        extension = extension.TrimStart('.');
        if (!_extensionMappings.TryGetValue(extension, out int colorValue)) {
            if (_index >= _shuffledNumbers.Length) {
                _index = 0;
            }
            colorValue = _shuffledNumbers[_index++];
            _extensionMappings[extension] = colorValue;
        }
        Program.ColorVals.Add(colorValue);
        Program.PathNames.Add(name);
    }
    // only checks the first letter, it's good enough
    private static void EvaluateLineItem(string name) {
        int i = 0;
        while (i < name.Length && !char.IsLetter(name[i])) {
            i++;
        }
        char letter = i == name.Length ? 'a' : char.ToLowerInvariant(name[i]);
        int letterValue = letter - 'a';
        Program.PathNames.Add(name);
        Program.ColorVals.Add(letterValue);
    }
}