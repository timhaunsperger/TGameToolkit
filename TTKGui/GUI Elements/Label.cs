﻿using OpenTK.Mathematics;
using TTKGui.Windowing;

namespace TTKGui.GUI_Elements;

public class Label : Element
{
    private int _labelSize;
    
    public Label(GuiWindow window, Vector2i pos, string label, int labelSize = 14, AlignMode align = AlignMode.CenterLeft) 
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