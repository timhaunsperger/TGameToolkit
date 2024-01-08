using System.Security.Cryptography.X509Certificates;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using StbImageSharp;
using TTKGui.GUI_Elements;

namespace TTKGui;
public class Texture
{
    public int Handle;
    public int Width;
    public int Height;
    public Texture(string path)
    {
        Handle = GL.GenTexture();
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
        
        // Flip image vertically because in OpenGL up is positive not negative on the y-axis
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

        Width = image.Width;
        Height = image.Height;
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba, 
            image.Width, 
            image.Height, 
            0, 
            PixelFormat.Rgba, 
            PixelType.UnsignedByte,
            image.Data);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }
    
    public Texture(byte[] imageData, int width, int height, PixelFormat pixelFormat = PixelFormat.Rgba)
    {
        Handle = GL.GenTexture();
        Width = width;
        Height = height;
        GL.BindTexture(TextureTarget.Texture2D, Handle);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureBorderColor, new float[]{0,0,0,0});
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba, 
            width, 
            height, 
            0, 
            pixelFormat, 
            PixelType.UnsignedByte,
            imageData);
    }

    public static Texture Box(Vector4i color, int width, int height, Vector4i? outlineColor = null, int outlineWidth = 0)
    {
        var length = width * height * 4;
        var cOutline = outlineColor ?? (255,255,255,255); // White default
        
        byte[] colorBytes = { (byte)color.X, (byte)color.Y, (byte)color.Z, (byte)color.W };
        byte[] cOutlineBytes = { (byte)cOutline.X, (byte)cOutline.Y, (byte)cOutline.Z, (byte)cOutline.W };
        byte[] data = new byte[length];
        
        // Fill box
        for (int i = 0; i < length; i += 4)
        {
            Array.Copy(colorBytes, 0, data, i, 4);
        }
        
        // Top outline
        for (int i = 0; i < outlineWidth * width * 4; i += 4)
        {
            Array.Copy(cOutlineBytes, 0, data, i, 4);
        }
        
        // Bottom outline
        for (int i = length - outlineWidth * width * 4; i < length; i += 4)
        {
            Array.Copy(cOutlineBytes, 0, data, i, 4);
        }
        
        // Side outlines
        for (int i = (width - outlineWidth) * 4; i < length - 8 * outlineWidth; i += width * 4)
        {
            for (int j = 0; j < outlineWidth * 8; j += 4)
            {
                Array.Copy(cOutlineBytes, 0, data, i + j, 4);
            }
        }
        
        return new Texture(data, width, height);
    }
    
    public static Texture Box(Vector4i color, Vector2i size, Vector4i? outlineColor = null, int outlineWidth = 0)
    {
        
        return Box(color, size.X, size.Y, outlineColor, outlineWidth);
    }
    
    public static Texture Circle(Vector4i color, int radius, Vector4i? outlineColor = null, int outlineWidth = 0)
    {
        var diameter = radius * 2;
        byte[] data = new byte[diameter * diameter * 4];
        byte[] colorBytes = {(byte)color.X, (byte)color.Y, (byte)color.Z, (byte)color.W};
        var radSqr = Math.Pow(radius, 2);
        
        for (int row = 0; row < diameter; row++)
        {
            var rowDistSqr = Math.Pow(row - radius, 2);
            for (int column = 0; column < diameter; column++)
            {
                var distSqr = rowDistSqr + Math.Pow(column - radius, 2);
                
                if (outlineColor != null && distSqr < radSqr - outlineWidth)
                {
                    data[row * diameter * 4 + column * 4] = (byte)outlineColor.Value.X;
                    data[row * diameter * 4 + column * 4 + 1] = (byte)outlineColor.Value.Y;
                    data[row * diameter * 4 + column * 4 + 2] = (byte)outlineColor.Value.Z;
                    data[row * diameter * 4 + column * 4 + 3] = (byte)outlineColor.Value.W;
                }
                else if (distSqr < radSqr)
                {
                    data[row * diameter * 4 + column * 4] = colorBytes[0];
                    data[row * diameter * 4 + column * 4 + 1] = colorBytes[1];
                    data[row * diameter * 4 + column * 4 + 2] = colorBytes[2];
                    data[row * diameter * 4 + column * 4 + 3] = colorBytes[3];
                }
                
            }
        }

        return new Texture(data, radius * 2, radius * 2);
    }

    public static Texture Cross(int width, int height, int lineWidth, Vector4i? color = null, bool blend = true)
    {
        var length = width * height * 4;
        var c = color ?? Theme.Text; // White default
        
        byte[] colorBytes = { (byte)c.X, (byte)c.Y, (byte)c.Z, (byte)c.W };
        byte[] blendBytes = { (byte)c.X, (byte)c.Y, (byte)c.Z, (byte)(c.W / 2) };
        byte[] data = new byte[length];
        
        // Fill box
        for (int i = 0; i < length; i += 4)
        {
            var pixInd = i / 4;
            var row = pixInd / (width);
            var column = pixInd % (width);

            if (blend && (Math.Abs(row - column) == lineWidth || Math.Abs(row - (width - column)) == lineWidth))
            {
                Array.Copy(blendBytes, 0, data, i, 4);
            }
            
            if (Math.Abs(row - column) < lineWidth || Math.Abs(row - (width - column)) < lineWidth)
            {
                Array.Copy(colorBytes, 0, data, i, 4);
            }
        }

        return new Texture(data, width, height);
    }
    
    /// <summary>
    /// Gets a texture with one blank pixel.
    /// </summary>
    public static readonly Texture Blank = new (new byte[]{0,0,0,0}, 1, 1);
    
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
    
}