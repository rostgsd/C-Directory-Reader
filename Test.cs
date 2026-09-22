namespace Directory_Reader;

public class Test {
    static void MainTest() {
        FileInfo fileInfo_buffer;
        var filePath = @"C:\Users\ostia\RiderProjects\Small Projects\Directory Reader\Directory Reader.csproj";
        fileInfo_buffer = new FileInfo(filePath);
        
        // File properties using FileInfo class
        Console.WriteLine("File name {0}", fileInfo_buffer.Name);
        Console.WriteLine("File size {0}", fileInfo_buffer.Length);
        Console.WriteLine("Date Created {0}", fileInfo_buffer.CreationTime);
        Console.WriteLine("File Extension {0}", fileInfo_buffer.Extension);
        
        // using color gradient 
        var gradientActor = new ColorGradient(PresetColors.Yellow, PresetColors.Blue, 202);
        gradientActor.GenerateGradient();
        
        // convert sample values to gradient
        int[] sampleData = { 12, 25, 1, 8, 4, 16, 18, 0, 22, 20, 3 };
        foreach (var data in sampleData) {
            RGB color = gradientActor.RgbFromRawInt(data, sampleData.Max());
            SetForegroundColor(color);
            Console.Write("  {0}  ", data);
            ResetForegroundColor();
        }
        
        // output contents of a folder
        DirectoryInfo dir = new(@"C:\Users\ostia\RiderProjects\Small Projects");
        OutputDirectory(dir);
    }
    // utility
    static void SetForegroundColor(RGB color) {
        Console.Write("\x1b[38;2;{0};{1};{2}m", color.R, color.G, color.B);
    }
    static void ResetForegroundColor() {
        Console.Write("\x1b[0m\n");
    }
    
    // loop through directory
    static int _recursionDepth = -1;
    static void IndentOnRecursion(int depth) {
        for (int i = 0; i < depth; i++) {
            Console.Write("   ");
        }
    }
    static void OutputDirectory(DirectoryInfo directoryInfo) {
        _recursionDepth++;
        // record current directory
        Console.WriteLine($"--{directoryInfo.Name}--");
        foreach (var fileInfo in directoryInfo.GetFiles()) {
            IndentOnRecursion(_recursionDepth);
            Console.WriteLine(fileInfo.Name);
        }
        foreach (var subDirectoryInfo in directoryInfo.GetDirectories()) {
            OutputDirectory(subDirectoryInfo);
        }
        _recursionDepth--;
    }
}