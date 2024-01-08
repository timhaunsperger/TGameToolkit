using OpenTK.Mathematics;
using TTKGui.Windowing;

namespace TTKGui.GUI_Elements;

public class Label : Element
{
    public Label(GuiWindow window, Vector2i pos, string label, int labelSize = 14, AlignMode align = AlignMode.CenterLeft) 
        : base(window, pos, align:align)
    {
        UpdateTexture(TextGenerator.GetStringTex(label, labelSize, Theme.Text));
    }
}