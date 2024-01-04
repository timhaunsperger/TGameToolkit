using System.Diagnostics;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TGUI.GUI_Elements;
using Debug = TGUI.GUI_Elements.Debug;

namespace TGUI.Windowing;



public class GuiWindow : GameWindow
{
    public List<Element> RootElements = new ();
    public int GuiScale;
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
        var timer = new Stopwatch();
        timer.Start();
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        foreach (var element in RootElements)
        {
            element.Draw();
        }
        Debug.DebugDraw();
        Context.SwapBuffers();
        base.OnRenderFrame(args);
        if (1 / UpdateTime < 155)
        {
            //Console.WriteLine(1/UpdateTime);
        }
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
        //RootElements.Add(new Panel(this, (50,50), (250, 400), "test panel"));
        //RootElements.Add(new Button(this, (100,100), (100, 20), "test"));
        RootElements.Add(new TextBox(this, (100,100), (300, 40), textSize:20));
    }
}