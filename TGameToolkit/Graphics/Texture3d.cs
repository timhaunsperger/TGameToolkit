using OpenTK.Graphics.OpenGL4;

namespace TGameToolkit.Graphics;

public class Texture3d
{
    public int Handle;
    public int Width;
    public int Height;
    
    public Texture3d(float[] imageData, int width, int height, int depth, PixelFormat pixelFormat = PixelFormat.Rgba)
    {
        Handle = GL.GenTexture();
        Width = width;
        Height = height;
        GL.BindTexture(TextureTarget.Texture3D, Handle);
        GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture3D,TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture3D,TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture3D,TextureParameterName.TextureBorderColor, new float[]{0,0,0,0});
        GL.TexImage3D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            width,
            height,
            depth,
            0,
            pixelFormat,
            PixelType.Float,
            imageData);
    }
    
    public Texture3d(int width, int height, int depth,
        PixelInternalFormat internalFormat = PixelInternalFormat.Rgba,
        PixelFormat pixelFormat = PixelFormat.Rgba, 
        TextureWrapMode wrapBehavior = TextureWrapMode.ClampToEdge)
    {
        Handle = GL.GenTexture();
        GL.Enable(EnableCap.Texture3DExt);
        Width = width;
        Height = height;
        GL.BindTexture(TextureTarget.Texture3D, Handle);
        GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture3D,TextureParameterName.TextureWrapS, (int)wrapBehavior);
        GL.TexParameter(TextureTarget.Texture3D,TextureParameterName.TextureWrapR, (int)wrapBehavior);
        GL.TexParameter(TextureTarget.Texture3D,TextureParameterName.TextureWrapT, (int)wrapBehavior);
        GL.TexImage3D(
            TextureTarget.Texture3D,
            0,
            internalFormat, 
            width, 
            height, 
            depth,
            0, 
            pixelFormat, 
            PixelType.Float,
            0);
    }
    
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture3D, Handle);
    }
}