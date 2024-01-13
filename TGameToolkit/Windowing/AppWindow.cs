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
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;

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
        Scene.Resize(ClientSize);
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
        if (RootElements.Count != 0)
        {
            foreach (var element in DisposedElements)
            {
                RootElements.Remove(element);
            }
            DisposedElements.Clear();
        }
        
        
        foreach (var obj in Scene.GameObjects)
        {
            obj.Update(args.Time);
        }

        if (!Paused)
        {
            Scene.GameCamera.Move(KeyboardState, (float)args.Time);
        }
        
        Debug.DebugDraw();
        Scene.DrawScene();
        
        GL.Disable(EnableCap.DepthTest);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        foreach (var element in RootElements)
        {
            element.Draw();
        }

        Context.SwapBuffers();
        var e = GL.GetError();
        if (e != ErrorCode.NoError)
        {
            Console.Write(e);
        }
        
        base.OnRenderFrame(args);
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        Scene.Resolution = ClientSize;
        Scene.Load();
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.ClearColor(0,0,0,0);
    }
}