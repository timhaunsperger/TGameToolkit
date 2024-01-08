using OpenTK.Mathematics;

namespace TTKGui.GUI_Elements;

public static class Theme
{
    public static Vector4i Base = (40, 40, 40, 255);
    public static Vector4i Accent = (80, 80, 255, 255);
    public static Vector4i Background = (30, 30, 30, 255);
    public static Vector4i Highlight1 = (60, 60, 60, 255);
    public static Vector4i Highlight2 = (80, 80, 80, 255);
    public static Vector4i Title = (30, 30, 60, 255);
    public static Vector4i Text = (255, 255, 255, 255);
    public static Vector4i Debug = (255, 0, 0, 255);
    
    public static readonly Vector4i Blank = (0, 0, 0, 0);
    public static readonly Vector4i White = (255, 255, 255, 255);
    public static readonly Vector4i Red = (255, 0, 0, 255);
    public static readonly Vector4i Green = (0, 255, 0, 255);
    public static readonly Vector4i Blue = (0, 0, 255, 255);
}