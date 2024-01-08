using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TTKGui.Windowing;

namespace TTKGui.GUI_Elements;

public class Slider : Element
{
    public float Value
    {
        get => _val;
        set
        {
            _val = value;
            UpdateSlider();
        }
    }

    private float _val;
    private float _min;
    private float _max;
    private bool _intSteps;
    public Action? OnUpdate;
    
    public Slider(
        GuiWindow window, Vector2i pos, Vector2i size, 
        float min,
        float max,
        float defVal,
        Texture? markerTex = null,
        Vector4i? slideColor = null,
        Vector4i? barColor = null,
        bool intSteps = true,
        
        AlignMode align = AlignMode.Default)
        : base(window, pos, Shader.BasicShader, Texture.Blank, align, size)
    {
        _intSteps = intSteps;
        _min = min;
        _max = max;
        _val = defVal;

        OnMouseClick = ClickAction;
        OnMouseDrag = DragAction;
        OnMouseUp = MouseUpAction;
        
        markerTex ??= Texture.Box(Theme.White, (10, size.Y));
        var slideTex = Texture.Box(slideColor ?? Theme.Accent, size);
        var barTex = Texture.Box(barColor ?? Theme.Background, size);
        var markerPos = (int)(defVal / max * size.X);

        var centerY = GetCenterYOffset();
        AddChild("bar", new Element(
            window, (markerPos, centerY), Shader.BasicShader, barTex, AlignMode.CenterLeft, (size.X - markerPos, size.Y / 2)));
        AddChild("slideBar", new Element(
            window, (0, centerY), Shader.BasicShader, slideTex, AlignMode.CenterLeft, (markerPos, size.Y / 2)));
        AddChild("marker", new Element(
            window, (markerPos, centerY), Shader.BasicShader, markerTex, AlignMode.Center));
    }

    private void ClickAction(Element e, Vector2i mousePos, MouseButtonEventArgs m)
    {
        if (m.Button != MouseButton.Button1 || !BoundingBox.ContainsInclusive(mousePos)) return;
        Flags.Add("Active");
        
        float slidePos = (mousePos.X - BoundingBox.Min.X) / (float)Size.X;
        _val = slidePos * (_max - _min) + _min;
        if (_intSteps)
        {
            _val = (int)Value;
        }
        UpdateSlider();
    }
    
    private void DragAction(Element e, MouseMoveEventArgs m)
    {
        if (!Flags.Contains("Active")) 
            return;
        
        float slidePos = (m.X - BoundingBox.Min.X) / Size.X;
        _val = slidePos * (_max - _min) + _min;
        if (_intSteps)
        {
            _val = (int)_val;
        }
        UpdateSlider();
    }

    private void MouseUpAction(Element e, Vector2i mPos, MouseButtonEventArgs mArgs)
    {
        Flags.Remove("Active");
    }

    private void UpdateSlider()
    {
        _val = Math.Clamp(_val, _min, _max);
        var markerPos = (int)((Value - _min) / (_max - _min) * Size.X);
        
        var bar = Children["bar"];
        var slider = Children["slideBar"];
        var marker = Children["marker"];
        
        marker.SetPos((markerPos, marker.Pos.Y));
        bar.SetPos((markerPos, marker.Pos.Y));
        bar.Resize(Size.X - markerPos, bar.Size.Y);
        slider.Resize(markerPos, slider.Size.Y);
        
        OnUpdate?.Invoke();
    }
}