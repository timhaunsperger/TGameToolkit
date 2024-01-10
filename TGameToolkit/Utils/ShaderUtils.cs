using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Drawing;

namespace TGameToolkit.Utils;

public static class ShaderUtils
{
    public static void FillVertexArrayAttribute(string attribName, Shader shader, ref double[] vertices, double[] data)
    {
        var stride = shader.AttribStride;
        var offset = shader.Attributes[attribName].Offset;
        var size = shader.Attributes[attribName].Size;
        
        if (data.Length / size < vertices.Length / stride)
        {
            Console.WriteLine("too small");
        }
        
        for (int i = 0; i < vertices.Length / stride; i++)
        {
            Array.Copy(data, i * size, vertices, i * stride + offset, size);
        }
    }
    
    public static void FillVertexArrayAttribute(string attribName, Shader shader, ref double[] vertices, Vector3d[] data)
    {
        
        var stride = shader.AttribStride;
        var offset = shader.Attributes[attribName].Offset;
        if (shader.Attributes[attribName].Size != 3)
        {
            Console.WriteLine(shader.Attributes[attribName].Size + "bad size");
        }

        if (data.Length < vertices.Length / stride)
        {
            Console.WriteLine("too small");
        }
        
        for (int i = 0; i < vertices.Length / stride; i++)
        {
            vertices[i * stride + offset] = data[i].X;
            vertices[i * stride + offset+1] = data[i].Y;
            vertices[i * stride + offset+2] = data[i].Z;
        }
    }
    
    public static void FillVertexArrayAttribute(string attribName, Shader shader, ref double[] vertices, Vector2d[] data)
    {
        
        var stride = shader.AttribStride;
        var offset = shader.Attributes[attribName].Offset;
        if (shader.Attributes[attribName].Size != 2)
        {
            Console.WriteLine(shader.Attributes[attribName].Size + "bad size");
        }
        
        if (data.Length < vertices.Length / stride)
        {
            Console.WriteLine("too small");
        }
        
        for (int i = 0; i < vertices.Length / stride; i++)
        {
            vertices[i * stride + offset] = data[i].X;
            vertices[i * stride + offset+1] = data[i].Y;
        }
    }
    
    public static void SetVertexAttribute(string attribName, Shader shader, ref double[] vertices, double[] data, int vertIndex)
    {
        
        var stride = shader.AttribStride;
        var offset = shader.Attributes[attribName].Offset + vertIndex * stride;
        var size = shader.Attributes[attribName].Size;
        
        for (int i = 0; i < size; i++)
        {
            vertices[offset + i] = data[i];
        }
    }
    /// <summary>
    /// Gets number of elements in type.
    /// </summary>
    public static int GetTypeComponentNum(ActiveAttribType type)
    {
        return type switch
        {
            ActiveAttribType.None => 0,
            ActiveAttribType.Int => 1,
            ActiveAttribType.UnsignedInt => 1,
            ActiveAttribType.Float => 1,
            ActiveAttribType.Double => 1,
            ActiveAttribType.FloatVec2 => 2,
            ActiveAttribType.FloatVec3 => 3,
            ActiveAttribType.FloatVec4 => 4,
            ActiveAttribType.IntVec2 => 2,
            ActiveAttribType.IntVec3 => 3,
            ActiveAttribType.IntVec4 => 4,
            ActiveAttribType.FloatMat2 => 4,
            ActiveAttribType.FloatMat3 => 9,
            ActiveAttribType.FloatMat4 => 16,
            ActiveAttribType.FloatMat2x3 => 6,
            ActiveAttribType.FloatMat2x4 => 8,
            ActiveAttribType.FloatMat3x2 => 6,
            ActiveAttribType.FloatMat3x4 => 12,
            ActiveAttribType.FloatMat4x2 => 8,
            ActiveAttribType.FloatMat4x3 => 12,
            ActiveAttribType.UnsignedIntVec2 => 2,
            ActiveAttribType.UnsignedIntVec3 => 3,
            ActiveAttribType.UnsignedIntVec4 => 4,
            ActiveAttribType.DoubleMat2 => 4,
            ActiveAttribType.DoubleMat3 => 9,
            ActiveAttribType.DoubleMat4 => 16,
            ActiveAttribType.DoubleMat2x3 => 6,
            ActiveAttribType.DoubleMat2x4 => 8,
            ActiveAttribType.DoubleMat3x2 => 6,
            ActiveAttribType.DoubleMat3x4 => 12,
            ActiveAttribType.DoubleMat4x2 => 8,
            ActiveAttribType.DoubleMat4x3 => 12,
            ActiveAttribType.DoubleVec2 => 2,
            ActiveAttribType.DoubleVec3 => 3,
            ActiveAttribType.DoubleVec4 => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}