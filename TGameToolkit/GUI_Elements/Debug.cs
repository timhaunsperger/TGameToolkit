using OpenTK.Mathematics;
using TGameToolkit.Drawing;
using TGameToolkit.Windowing;

namespace TGameToolkit.GUI_Elements;

public static class Debug
{
    public static List<Element> DebugElements = new ();
    public static void DrawRect(AppWindow window, Vector2i min, Vector2i max)
    {
        var rect = new Element(
            window, 
            min, 
            Shader.UiShader, 
            Texture.Box((0,0,0,0), max - min, Theme.Debug, 1));
        DebugElements.Add(rect);
    }
    
    public static void DrawRect(AppWindow window, Box2i box)
    {
        DrawRect(window, box.Min, box.Max);
    }

    public static void OutlineElement(Element e)
    {
        DebugElements.Add(e);
    }

    public static void DebugDraw()
    {
        foreach (var e in DebugElements)
        {
            var eTex = e.Tex;
            var debugTex = Texture.Box(Theme.Blank, eTex.Width, eTex.Height, Theme.Debug, 1);
            e.UpdateTexture(debugTex);
            e.Render();
            e.UpdateTexture(eTex);
        }
    }
}