using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TGameToolkit.Attributes;
using TGameToolkit.Drawing;
using TGameToolkit.GUI_Elements;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;
using Debug = TGameToolkit.GUI_Elements.Debug;

namespace TGameToolkit.Windowing;



public class AppWindow : GameWindow
{
    public List<Element> RootElements = new ();
    public List<Element> DisposedElements = new ();

    

    public bool Paused = false;
    
    public AppWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings) 
        : base(gameSettings, nativeSettings)
    {
        Scene.GameCamera.AspectRatio = ClientSize.X / (float)ClientSize.Y;
        //CursorState = CursorState.Grabbed;
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        foreach (var element in RootElements)
        {
            element.UpdateVertices();
        }

        Scene.GameCamera.AspectRatio = ClientSize.X / (float)ClientSize.Y;
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
            Scene.GameCamera.MouseMove(m.Delta);
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
        foreach (var obj in Scene.GameObjects)
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
            Scene.GameCamera.Move(KeyboardState, (float)args.Time);
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
        // var rand = new Random();
        // for (int i = 0; i < 1; i++)
        // {
        //     var light = new GameObject(){Pos = (MathF.Cos(i) * 5, (float)rand.NextDouble() * 5f, MathF.Sin(i) * 5)};
        //     Scene.GameObjects.Add(light);
        //     new PointLight(light);
        //     new CubePrimitive(
        //         light,
        //         new Shader("Shaders/basic.vert", "Shaders/lighting.frag"),
        //         new Material() { Tex = Texture.Box((255, 255, 255, 255), 100, 100), AmbientStrength = 10 });
        // }
        //
        
        // var cube2 = new GameObject(){Pos = (0, -2, 3)};
        // new CubePrimitive(
        //     cube2, 
        //     new Shader("Shaders/basic.vert", "Shaders/lighting.frag"),
        //     new Material());
        // Scene.GameObjects.Add(cube2);
    }
}