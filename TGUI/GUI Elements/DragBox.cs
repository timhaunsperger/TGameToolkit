using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TGUI.Windowing;

namespace TGUI.GUI_Elements;

public class DragBox : Element
{
    public DragBox(GuiWindow window, Vector2i pos, Vector2i size, AlignMode align = AlignMode.Default) 
        : base(window, pos, size:size, align:align)
    {
        Flags.Add("Invisible");
        OnMouseMove = MouseMoveAction;
        OnMouseUp = MouseUpAction;
        OnMouseClick = MouseClickAction;
    }

    private void MouseClickAction(Element e, Vector2i mPos, MouseButtonEventArgs m)
    {
        if (m.Button == MouseButton.Button1 && BoundingBox.ContainsInclusive(mPos))
        {
            Flags.Add("Active");
        }
    }
    private void MouseUpAction(Element e, Vector2i mPos, MouseButtonEventArgs m)
    {
        if (m.Button == MouseButton.Button1)
        {
            Flags.Remove("Active");
        }
    }

    private void MouseMoveAction(Element e, MouseMoveEventArgs m, MouseState mouseState)
    {
        if (Flags.Contains("Active"))
        {
            Parent?.Move((Vector2i)m.Delta);
        }
    }
}
