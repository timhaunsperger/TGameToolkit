﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using TGameToolkit.Drawing;
using TGameToolkit.Windowing;

namespace TGameToolkit.GUI_Elements;

public class Checkbox : Element
{
    public bool IsChecked = false;
    
    public Texture CheckTex;
    public Texture BaseTex;
    public Action OnPress = () => { };
    
    public Checkbox( 
        GuiWindow window, Vector2i pos, int size, Vector4i? color = null,
        AlignMode align = AlignMode.Default) 
        : base(window, pos, Shader.UiShader, Texture.Blank, align, (size, size))
    {
        BaseTex = Texture.Box(color ?? Theme.Base, size, size, Theme.Highlight2, 1 );
        CheckTex = Texture.Box(Theme.White, size / 2, size / 2, Theme.Blank, size / 10);
        
        UpdateTexture(BaseTex);
        
        OnMouseClick = ClickAction;
        OnMouseUp = MouseUpAction;
    }
    
    private void ClickAction(Element e, Vector2i pos, MouseButtonEventArgs m)
    {
        if (!BoundingBox.ContainsInclusive(pos)) return;
        Flags.Add("Active");
    }
    
    private void MouseUpAction(Element e, Vector2i pos, MouseButtonEventArgs m)
    {
        if (!Flags.Contains("Active")) return;
        Flags.Remove("Active");
        
        if (!BoundingBox.ContainsInclusive(pos)) return;
        OnPress.Invoke();
        
        if (IsChecked)
        {
            IsChecked = false;
            UpdateTexture(BaseTex);
            return;
        }

        IsChecked = true;
        UpdateTexture(CheckTex);
    }
    
}