using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TGameToolkit.Attributes;
using TGameToolkit.Drawing;
using TGameToolkit.GUI_Elements;
using TGameToolkit.Objects;
using Debug = TGameToolkit.GUI_Elements.Debug;

namespace TGameToolkit.Windowing;



public class GuiWindow : GameWindow
{
    public List<Element> RootElements = new ();
    public List<Element> DisposedElements = new ();

    public List<GameObject> GameObjects = new ();
    public readonly Camera GameCamera;

    public bool Paused = false;
    
    public GuiWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings) 
        : base(gameSettings, nativeSettings)
    {
        GameCamera = new Camera(Vector3.Zero, ClientSize.X / (float)ClientSize.Y);
        //CursorState = CursorState.Grabbed;
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        foreach (var element in RootElements)
        {
            element.UpdateVertices();
        }

        GameCamera.AspectRatio = ClientSize.X / (float)ClientSize.Y;
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        foreach (var element in RootElements)
        {
            element.OnKeyInput.Invoke(element, e);
        }

        if (e.Key == Keys.Escape )
        {
            Paused = !Paused;
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
        
        if (IsFocused && !Paused)
        {
            GameCamera.MouseMove(m.Delta);
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
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        if (RootElements.Count != 0)
        {
            foreach (var element in DisposedElements)
            {
                RootElements.Remove(element);
            }
            DisposedElements.Clear();
        }
        
        GL.Enable(EnableCap.DepthTest);
        foreach (var obj in GameObjects)
        {
            obj.Update(args.Time);
        }
        
        GL.Disable(EnableCap.DepthTest);
        foreach (var element in RootElements)
        {
            element.Draw();
        }

        if (!Paused)
        {
            GameCamera.Move(KeyboardState, (float)args.Time);
        }
        
        Debug.DebugDraw();
        base.OnRenderFrame(args);
        
        Context.SwapBuffers();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        // var panel = new Panel(this, (0, 0), 250, "Elements");
        // panel.AddElement(new Button(this, Vector2i.Zero, (40, panel.SlotHeight), "test"), "Test Button1");
        // panel.AddElement(new Button(this, Vector2i.Zero, (40, panel.SlotHeight), "test"), "Button2");
        // panel.AddElement(new Slider(this, Vector2i.Zero, (100, panel.SlotHeight), 0, 10, 5), "slider???");
        // panel.AddElement(new TextBox(this, Vector2i.Zero, (100, panel.SlotHeight)), "TEXT BOX");
        // panel.AddElement(new Checkbox(this, Vector2i.Zero, panel.SlotHeight), "checkbox");
        // RootElements.Add(panel);
        
        var cube = new GameObject();
        GameObjects.Add(cube);
        var a = new CubePrimitive(
            cube, GameCamera, 
            new Shader("Shaders/basic.vert", "Shaders/basic.frag"), 
            Texture.Box(Theme.Red, 100, 100));

    }
}