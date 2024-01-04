using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using TGUI.Windowing;

namespace TGUI.GUI_Elements;

public class Button : Element
{
    public Texture? HoverTex;
    public Texture BaseTex;
    public Action OnPress = () => { };
    
    public Button( 
        GuiWindow window, Vector2i pos, Vector2i size, string label = "", Vector4i? color = null,
        AlignMode align = AlignMode.Default, int labelSize = 0) 
        : base(window, pos, Shader.BasicShader, Texture.Blank, align, size)
    {
        BaseTex = Texture.Box(color ??Theme.BaseColor, size);
        HoverTex = Texture.Box(Theme.HighlightColor, size);
        
        UpdateTexture(BaseTex);
        if (label != "")
        {
            var labelTex = TextGenerator.GetStringTex(label, labelSize == 0 ? size.Y / 2 : labelSize, (255, 255, 255, 255));
            var labelPos = BoundingBox.Min + (size.X / 2, size.Y / 2) - pos;
            var labelElement = new Element(window, labelPos, Shader.BasicShader, labelTex, align: AlignMode.Center);
            AddChild("label", labelElement);
        }
        
        OnMouseEnter += MouseEnterAction;
        OnMouseExit = MouseLeaveAction;
        OnMouseClick = ClickAction;
    }

    private void MouseEnterAction(Element e)
    {
        if (HoverTex != null)
        {
            UpdateTexture(HoverTex);
        }
    }
    
    private void MouseLeaveAction(Element e)
    {
        if (HoverTex != null)
        {
            UpdateTexture(BaseTex);
        }
    }
    
    private void ClickAction(Element e, Vector2i pos, MouseButtonEventArgs m)
    {
        if (!BoundingBox.ContainsInclusive(pos)) return;
        OnPress.Invoke();
    }
    
}