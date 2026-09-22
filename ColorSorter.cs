namespace Directory_Reader;

public static class PresetColors {
    public static readonly RGB Red = new (255, 0, 0);
    public static readonly RGB Green = new RGB(0, 255, 0);
    public static readonly RGB Blue = new RGB(0, 0, 255);
    public static readonly RGB Yellow = new RGB(255, 255, 0);
    public static readonly RGB Magenta = new RGB(255, 0, 255);
    public static readonly RGB Cyan = new RGB(0, 255, 255);
    public static readonly RGB White = new RGB(255, 255, 255);
    public static readonly RGB Grey = new RGB(128, 128, 128);
    // etc...
}
public struct RGB {
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    public RGB() { }
    public RGB(byte r, byte g, byte b) {
        R = r;
        G = g;
        B = b;
    }
}
// only one instance necessary
public class ColorGradient {
    public RGB StartColor { get; set; }
    public RGB EndColor { get; set; }
    public int SampleSize { get; set; }
    private List<RGB> _pallete;

    public ColorGradient(RGB startColor, RGB endColor) : this(startColor, endColor, 16) { }
    public ColorGradient(RGB startColor, RGB endColor, int sampleSize) {
        if (sampleSize < 0 || sampleSize > 255) {
            throw new ArgumentOutOfRangeException(nameof(sampleSize));
        }
        StartColor = startColor;
        EndColor = endColor;
        SampleSize = sampleSize;
        _pallete = new();
    }

    public void GenerateGradient() {
        _pallete.Clear();
        
        byte LinearInterpolate(double value, byte min, byte max) {
            value = (max - min)/(double)SampleSize*value + min;
            value = Math.Floor(value);
            return (byte)value;
        }
        for (int i = 0; i < SampleSize; i++) {
            var color = new RGB(
                LinearInterpolate(i,StartColor.R,EndColor.R),
                LinearInterpolate(i,StartColor.G,EndColor.G),
                LinearInterpolate(i,StartColor.B,EndColor.B)
            );
            _pallete.Add(color);
        }
    }

    public RGB RgbFromRawInt(int value, int maxValue) {
        double lookupValue = Math.Floor(value * (double)SampleSize / maxValue);
        if ((int)lookupValue == SampleSize) {
            lookupValue -= 1;
        }
        return _pallete[(int)lookupValue];
    }
}



