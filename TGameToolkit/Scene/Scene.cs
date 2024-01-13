using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Attributes;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;

namespace TGameToolkit;

public static class Scene
{
    public static List<GameObject> GameObjects = new();
    public static Camera GameCamera = new Camera();
    
    public static List<PointLight> Lights = new();
    public static DirectionalLight? GlobalLight;
    
    public static Vector2i Resolution;
    // Lists render passes used to acquire scene data for use in post-processing
    public static RenderPass GeometryPass;
    public static Postprocessor? ScenePostprocessor;
    public static bool IsLoaded;

    public static void Load()
    {
        GeometryPass = new RenderPass();
        ScenePostprocessor ??= new Postprocessor(new Shader(Postprocessor.BaseVtxShaderSrc, Postprocessor.BaseFragShaderSrc));
        IsLoaded = true;
    }
    
    public static void DrawScene()
    {
        if (!IsLoaded) return;
        // Get scene data
        GeometryPass.Run();
        
        // Render Scene
        ScenePostprocessor?.Use(0);
        
    }

    public static void Resize(Vector2i newSize)
    {
        if (!IsLoaded) return;
        
        var x = newSize.X;
        var y = newSize.Y;
        
        GameCamera.AspectRatio = (float)x / y;
        Resolution = new Vector2i(x, y);
        GL.Viewport(0, 0, x, y);

        GeometryPass.Regenerate();
    }

}