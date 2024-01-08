using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TTKGui.GUI_Elements;
using Debug = TTKGui.GUI_Elements.Debug;

namespace TTKGui.Windowing;



public class GuiWindow : GameWindow
{
    public List<Element> RootElements = new ();
    public List<Element> DisposedElements = new ();
    public GuiWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings) 
        : base(gameSettings, nativeSettings)
    {
        
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        foreach (var element in RootElements)
        {
            element.UpdateVertices();
        }
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        foreach (var element in RootElements)
        {
            element.OnKeyInput.Invoke(element, e);
        }
        base.OnKeyDown(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        foreach (var element in RootElements)
        {
            element.OnMouseClick.Invoke(element, (Vector2i)MousePosition, e);
        }
        
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        foreach (var element in RootElements)
        {
            element.OnMouseUp.Invoke(element, (Vector2i)MousePosition, e);
        }
        
        base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseMoveEventArgs m)
    {
        foreach (var element in RootElements)
        {
            element.OnMouseMove.Invoke(element, m, MouseState);
        }
        
        base.OnMouseMove(m);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        foreach (var element in RootElements)
        {
            element.OnTextInput.Invoke(element, e.AsString);
        }
        base.OnTextInput(e);
    }
    
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        if (RootElements.Count != 0)
        {
            foreach (var element in DisposedElements)
            {
                RootElements.Remove(element);
            }
            DisposedElements.Clear();
        }
        
        foreach (var element in RootElements)
        {
            element.Draw();
        }
        Debug.DebugDraw();
        base.OnRenderFrame(args);
        
        Context.SwapBuffers();

    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(Color.Black);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // RootElements.Add(new TextBox(
        //     this,
        //     (Vector2i)ClientRectangle.Center,
        //     (600, 150),
        //     AlignMode.Center, textSize: 16));
        // for (int i = 0; i < 750; i++)
        // {
        //     
        // }
        //RootElements.Add(new Slider(this, (200,100), (100, 15), 0, 10, 2));
        var panel = new Panel(this, (100, 100), (250, 300), "Elements", align: AlignMode.UpperLeft);
        panel.AddElement(new Button(this, Vector2i.Zero, (40, panel.SlotHeight), "test"), "Test Button1");
        panel.AddElement(new Button(this, Vector2i.Zero, (40, panel.SlotHeight), "test"), "Button2");
        panel.AddElement(new Slider(this, Vector2i.Zero, (100, panel.SlotHeight), 0, 10, 5), "slider???");
        panel.AddElement(new TextBox(this, Vector2i.Zero, (100, panel.SlotHeight)), "TEXT BOX");
        panel.AddElement(new Checkbox(this, Vector2i.Zero, panel.SlotHeight), "checkbox");
        RootElements.Add(panel);
        //RootElements[0].AnchorPoint = (0.5f, 0.5f);
        
        //RootElements.Add(new Button(this, (100,100), (100, 20), "test"));
        //RootElements.Add(new TextBox(this, (100,100), (300, 40), textSize:20));
    }
}