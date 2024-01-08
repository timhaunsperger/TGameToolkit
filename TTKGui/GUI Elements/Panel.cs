using OpenTK.Mathematics;
using TTKGui.Windowing;

namespace TTKGui.GUI_Elements;

public class Panel : Element
{
    public int SlotHeight => _slotSize - _padding;
    private readonly int _slotSize;
    private readonly int _divPos;
    private readonly int _padding = 5;
    private int _netElementHeight;
    
    public Panel(GuiWindow window, Vector2i pos, Vector2i size, string title, AlignMode align = AlignMode.Default,
    Vector4i? titleBarColor = null, Vector4i? paneColor = null, int slotSize = 24) 
        : base(window, pos, size:size, align:align)
    {
        _divPos = size.X / 3;
        _slotSize = slotSize;

        var titleBarHeight = 24;
        _netElementHeight = titleBarHeight + _padding;
        
        //Set panel texture
        UpdateTexture(Texture.Box(paneColor ?? Theme.Base, size));
        
        // Get position of title bar
        var barPos = BoundingBox.Min - pos;
        
        // Create title bar element
        var titleBar = new Element(
            window, barPos, texture: Texture.Box(titleBarColor ?? Theme.Title, (size.X, titleBarHeight)), align:AlignMode.UpperLeft);
        
        // Add title bar
        var titleLabel = new Label(window, (_padding , titleBarHeight / 2), title, titleBarHeight / 2);
        titleBar.AddChild("Title", titleLabel);
        AddChild("titleBar", titleBar);
        
        // Add close button
        var buttonPos = BoundingBox.Min - pos + (size.X, titleBarHeight / 2);
        var closeButton = new Button(
            window,
            buttonPos,
            (titleBarHeight, titleBarHeight),
            "",
            Theme.Title,
            AlignMode.CenterRight)
        {
            HoverTex = Texture.Box(Theme.Red, (titleBarHeight, titleBarHeight)),
            OnPress = Dispose
        };
        closeButton.AddChild("label", new Element(
            window, ( - titleBarHeight / 4, 0), size: (titleBarHeight / 2, titleBarHeight / 2), 
            texture:Texture.Cross(titleBarHeight / 2, titleBarHeight / 2, 1), align:AlignMode.CenterRight));

        AddChild("close", closeButton);
        
        // Add drag box
        Vector2i dragBoxSize = (size.X - _slotSize - titleLabel.Size.X - _padding, _slotSize);
        AddChild("dragBox", new DragBox(window, barPos + (titleLabel.Size.X, 0), dragBoxSize));
        
        ForwardAllInteracts();
    }
    /// <summary>
    /// Adds element to next slot in panel with label. 
    /// </summary>
    public void AddElement(Element e, string label)
    {
        var slotPos = (_padding, _netElementHeight) - PosOffset;
        AddChild(label + "Label", new Label(
            Window, slotPos + (0, _slotSize / 2), label, _slotSize / 2));
        AddChild(label + "Divider", new Element(
            Window, slotPos + (_divPos, 0), texture:Texture.Box(Theme.Background, 2, _slotSize), align: AlignMode.CenterLeft));
        e.SetPos(slotPos + (_divPos + _padding, 0));
        
        AddChild(label, e);
        _netElementHeight += _slotSize;
    }
    
    // Overwrites all basic interaction actions with invocation of interaction on all children
    private void ForwardAllInteracts()
    {
        OnMouseClick = (e, mPos, mArgs) => { foreach (var child in e.Children.Values) child.OnMouseClick.Invoke(child, mPos, mArgs); };
        OnMouseUp = (e, mPos, mArgs) => { foreach (var child in e.Children.Values) child.OnMouseUp.Invoke(child, mPos, mArgs); };
        OnTextInput = (e, str) => { foreach (var child in e.Children.Values) child.OnTextInput.Invoke(child, str); };
        OnKeyInput = (e, key) => { foreach (var child in e.Children.Values) child.OnKeyInput.Invoke(child, key); };
        OnMouseMove = (e, mArgs, mState) => { foreach (var child in e.Children.Values) child.OnMouseMove.Invoke(child, mArgs, mState); };
        OnDraw = (e) => { foreach (var child in e.Children.Values) child.OnDraw.Invoke(child); };
    }
}