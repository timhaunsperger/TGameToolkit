using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using TTKGui.Windowing;

namespace TTKGui.GUI_Elements;

public class Checkbox : Element
{
    public Texture CheckTex;
    public Texture BaseTex;
    public Action OnPress = () => { };
    
    public Checkbox( 
        GuiWindow window, Vector2i pos, int size, Vector4i? color = null,
        AlignMode align = AlignMode.Default) 
        : base(window, pos, Shader.BasicShader, Texture.Blank, align, (size, size))
    {
        BaseTex = Texture.Box(color ?? Theme.Base, size, size, Theme.Highlight2, 1 );
        CheckTex = Texture.Box(Theme.White, size / 2, size / 2, Theme.Blank, size / 10);
        
        UpdateTexture(BaseTex);
        
        OnMouseEnter = MouseEnterAction;
        OnMouseExit = MouseLeaveAction;
        OnMouseClick = ClickAction;
    }

    private void MouseEnterAction(Element e)
    {

    }
    
    private void MouseLeaveAction(Element e)
    {

    }
    
    private void ClickAction(Element e, Vector2i pos, MouseButtonEventArgs m)
    {
        if (!BoundingBox.ContainsInclusive(pos)) return;
        
        if (Flags.Contains("Active"))
        {
            Flags.Remove("Active");
            UpdateTexture(BaseTex);
            return;
        }
        Flags.Add("Active");
        UpdateTexture(CheckTex);
        
    }
    
}