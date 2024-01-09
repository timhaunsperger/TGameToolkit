using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using TGameToolkit.Drawing;
using TGameToolkit.GUI_Elements.Text;
using TGameToolkit.Windowing;

namespace TGameToolkit.GUI_Elements;

public class Button : Element
{
    public Texture ClickTex;
    public Texture HoverTex;
    public Texture BaseTex;
    public Action OnPress = () => { };
    
    public Button( 
        AppWindow window, Vector2i pos, Vector2i size, string label = "", Vector4i? color = null,
        AlignMode align = AlignMode.Default, int labelSize = 14) 
        : base(window, pos, Shader.UiShader, Texture.Blank, align, size)
    {
        BaseTex = Texture.Box(color ??Theme.Base, size);
        HoverTex = Texture.Box(Theme.Highlight1, size);
        ClickTex = Texture.Box(Theme.Highlight2, size);
        
        UpdateTexture(BaseTex);
        if (label != "")
        {
            var labelTex = TextGenerator.GetStringTex(label, labelSize == 0 ? size.Y / 2 : labelSize, (255, 255, 255, 255));
            var labelPos = BoundingBox.Min + (size.X / 2, size.Y / 2) - pos;
            var labelElement = new Element(window, labelPos, Shader.UiShader, labelTex, align: AlignMode.Center);
            AddChild("label", labelElement);
        }
        
        OnMouseEnter = MouseEnterAction;
        OnMouseExit = MouseLeaveAction;
        OnMouseClick = ClickAction;
        OnMouseUp = MouseUpAction;
    }

    private void MouseEnterAction(Element e)
    {
        UpdateTexture(HoverTex);
    }
    
    private void MouseLeaveAction(Element e)
    {
        UpdateTexture(BaseTex);
    }
    
    private void ClickAction(Element e, Vector2i pos, MouseButtonEventArgs m)
    {
        if (!BoundingBox.ContainsInclusive(pos)) return;
        Flags.Add("Active");
        UpdateTexture(ClickTex);
    }

    private void MouseUpAction(Element e, Vector2i pos, MouseButtonEventArgs m)
    {
        if (!Flags.Contains("Active")) return;
        Flags.Remove("Active");
        if (BoundingBox.ContainsInclusive(pos))
        {
            OnPress.Invoke();
            UpdateTexture(HoverTex);
            return;
        }
        UpdateTexture(BaseTex);
    }
    
}