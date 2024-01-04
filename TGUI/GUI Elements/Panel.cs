using OpenTK.Mathematics;
using TGUI.Windowing;

namespace TGUI.GUI_Elements;

public class Panel : Element
{
    public Panel(GuiWindow window, Vector2i pos, Vector2i size, string title, AlignMode align = AlignMode.Default,
    Vector4i? titleBarColor = null, Vector4i? paneColor = null) : base(window, pos, size:size, align:align)
    {
        //Set panel texture
        UpdateTexture(Texture.Box(paneColor ?? Theme.BackgroundColor, size));
        
        // Get position of title bar
        var barHeight = 20;
        var barPos = BoundingBox.Min - pos;
        
        // Create title bar element
        var titleBar = new Element(
            window, barPos, texture: Texture.Box(titleBarColor ?? Theme.TitleColor, (size.X, barHeight)), align:AlignMode.UpperLeft);
        
        // Add title bar
        var titleLabel = new Element(window, (5 , barHeight / 2), 
            texture:TextGenerator.GetStringTex(title, 14, Theme.TextColor), align:AlignMode.CenterLeft);
        titleBar.AddChild("Title", titleLabel);
        AddChild("titleBar", titleBar);
        
        // Add close button
        AddChild("close", new Button(
                window, 
                (size.X, barHeight / 2), 
                (barHeight, barHeight), 
                "\u00d7",
                Theme.TitleColor,
                AlignMode.CenterRight,
                barHeight)
                {
                    HoverTex = Texture.Box(Theme.Red, (barHeight, barHeight))
                }
        );
        
        // Add drag box
        Vector2i dragBoxSize = (size.X - barHeight - titleLabel.Size.X - 5, barHeight);
        AddChild("dragBox", new DragBox(window, barPos + (titleLabel.Size.X, 0), dragBoxSize));
        
        ForwardAllInteracts();
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