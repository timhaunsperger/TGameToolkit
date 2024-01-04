using OpenTK.Mathematics;

namespace TGUI.GUI_Elements;

public static class Theme
{
    public static Vector4i BaseColor = (50, 50, 50, 255);
    public static Vector4i AccentColor = (50, 30, 180, 255);
    public static Vector4i BackgroundColor = (30, 30, 30, 255);
    public static Vector4i HighlightColor = (60, 60, 60, 255);
    public static Vector4i TitleColor = (30, 30, 60, 255);
    public static Vector4i TextColor = (255, 255, 255, 255);
    public static Vector4i DebugColor = (255, 0, 0, 255);
    public static Vector4i BlankColor = (0, 0, 0, 0);

    public static readonly Vector4i White = (255, 255, 255, 255);
    public static readonly Vector4i Red = (255, 0, 0, 255);
    public static readonly Vector4i Green = (0, 255, 0, 255);
    public static readonly Vector4i Blue = (0, 0, 255, 255);
}