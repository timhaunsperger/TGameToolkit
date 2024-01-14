using OpenTK.Graphics.OpenGL4;
using TGameToolkit.Attributes;
using TGameToolkit.Drawing;
using TGameToolkit.Objects;

namespace TGameToolkit;

public class RenderPass
{
    private int _fbo;
    public int ColorTex;
    public int PosTex;
    public int DepthTex;
    public int NormalTex;
    public Shader? PassShader;

    public RenderPass(Shader? shader = null)
    {
        PassShader = shader;
        Init();
    }

    private void Init()
    {
        // Initialize framebuffer
        _fbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

        // Create color texture
        ColorTex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, ColorTex);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexImage2D(TextureTarget.Texture2D, 
            0, 
            PixelInternalFormat.Rgba, 
            Scene.Resolution.X, Scene.Resolution.Y, 0, 
            PixelFormat.Rgba, 
            PixelType.UnsignedByte, 
            0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTex, 0);

        // Create position texture
        PosTex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, PosTex);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexImage2D(TextureTarget.Texture2D, 
            0, 
            PixelInternalFormat.Rgba16f, 
            Scene.Resolution.X, Scene.Resolution.Y, 0, 
            PixelFormat.Rgba, 
            PixelType.Float, 
            0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, PosTex, 0);

        // Create depth texture
        DepthTex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, DepthTex);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexImage2D(TextureTarget.Texture2D, 
            0, 
            PixelInternalFormat.DepthComponent, 
            Scene.Resolution.X, Scene.Resolution.Y, 0, 
            PixelFormat.DepthComponent, 
            PixelType.Float, 
            0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTex, 0);
        
        // Create normal texture
        NormalTex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, NormalTex);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexImage2D(TextureTarget.Texture2D, 
            0, 
            PixelInternalFormat.Rgba, 
            Scene.Resolution.X, Scene.Resolution.Y, 0, 
            PixelFormat.Rgba, 
            PixelType.Float, 
            0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, NormalTex, 0);
        
        GL.DrawBuffers(3, new [] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 });
        Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
    }

    public void Regenerate()
    {
        GL.DeleteFramebuffer(_fbo);
        GL.DeleteTexture(ColorTex);
        GL.DeleteTexture(DepthTex);
        GL.DeleteTexture(PosTex);
        GL.DeleteTexture(NormalTex);
        Init();
    }

    public void Run()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        GL.ClearColor(0,0,0, 0);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);
        foreach (var obj in Scene.GameObjects)
        {
            obj.RenderMeshes(PassShader);
        }
    }
}