using OpenTK.Mathematics;
using TGameToolkit.GUI_Elements.Text;
using TGameToolkit.Windowing;
using TGameToolkit.Graphics;

namespace TGameToolkit.GUI_Elements;

public class Label : Element
{
    private int _labelSize;
    
    public Label(AppWindow window, Vector2i pos, string label, int labelSize = 14, AlignMode align = AlignMode.CenterLeft) 
        : base(window, pos, align:align)
    {
        _labelSize = labelSize;
        UpdateTexture(TextGenerator.GetStringTex(label, labelSize, Theme.Text));
    }
    
    public void UpdateLabel(string text)
    {
        UpdateTexture(TextGenerator.GetStringTex(text, _labelSize, Theme.Text));
    }
}