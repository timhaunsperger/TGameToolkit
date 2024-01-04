using OpenTK.Mathematics;
using TGUI.Windowing;

namespace TGUI.GUI_Elements;

public static class Debug
{
    public static List<Element> DebugElements = new ();
    public static void DrawRect(GuiWindow window, Vector2i min, Vector2i max)
    {
        var rect = new Element(
            window, 
            min, 
            Shader.BasicShader, 
            Texture.Box((0,0,0,0), max - min, Theme.DebugColor, 1));
        DebugElements.Add(rect);
    }
    
    public static void DrawRect(GuiWindow window, Box2i box)
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
            var eTex = e.Texture;
            var debugTex = Texture.Box(Theme.BlankColor, eTex.Width, eTex.Height, Theme.DebugColor, 1);
            e.UpdateTexture(debugTex);
            e.Render();
            e.UpdateTexture(eTex);
        }
    }
}