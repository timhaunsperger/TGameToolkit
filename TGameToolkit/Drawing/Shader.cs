using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Utils;

namespace TGameToolkit.Drawing;

public class Shader : IDisposable
{
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniformLocations = new ();
    
    /// <summary>
    /// Stores all vertex attributes, value format: (location (in shader), offset, size)
    /// </summary>
    public readonly Dictionary<string, AttribInfo> Attributes = new ();
    public readonly int AttribStride;

    public static readonly Shader UiShader = BuiltIn(
        "TGameToolkit.Shaders.UI.vert", "TGameToolkit.Shaders.UI.frag");
    public static readonly Shader LightingShader = BuiltIn(
        "TGameToolkit.Shaders.basic.vert", "TGameToolkit.Shaders.lighting.frag");

    private static Shader BuiltIn(string vertRsc, string fragRsc)
    {
        var assembly = Assembly.GetExecutingAssembly();
        StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(vertRsc)!);
        string vertSrc = reader.ReadToEnd();
        reader = new StreamReader(assembly.GetManifestResourceStream(fragRsc)!);
        string fragSrc = reader.ReadToEnd();
        return new Shader(vertSrc, fragSrc);
    }

    public static Shader GenShader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);
        return new Shader(vertexShaderSource, fragmentShaderSource);
    }
    
    private Shader(string vertexShaderSource, string fragmentShaderSource)
    {
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        string infoLogVert = GL.GetShaderInfoLog(vertexShader);
        if (infoLogVert != "")
            Console.WriteLine(infoLogVert);
        
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
        if (infoLogFrag != "")
            Console.WriteLine(infoLogFrag);
        
        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);
        
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
        
        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(_handle, i, out _, out _);
            var location = GL.GetUniformLocation(_handle, key);
            _uniformLocations.Add(key, location);
        }
        
        GL.GetProgram(_handle, GetProgramParameterName.ActiveAttributes, out var numAttributes);
        var offset = 0;
        for (var i = 0; i < numAttributes; i++)
        {
            var key = GL.GetActiveAttrib(_handle, i, out _, out var type);
            var location = GL.GetAttribLocation(_handle, key);
            var size = ShaderUtils.GetTypeComponentNum(type);
            if (key != null)
            {
                Attributes.Add(key, new AttribInfo(location, offset, size));
            }
            offset += size;
        }

        AttribStride = offset;
    }
    
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(_handle, attribName);
    }
    
    public void SetInt(string name, int data)
    {
        int location = GL.GetUniformLocation(_handle, name);

        GL.Uniform1(location, data);
    }
    
    public void SetFloat(string name, float data)
    {
        GL.UseProgram(_handle);
        GL.Uniform1(_uniformLocations[name], data);
    }
    
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(_handle);
        GL.Uniform3(_uniformLocations[name], data);
    }
    
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(_handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }
    
    public void Use()
    {
        GL.UseProgram(_handle);
    }
    
    private bool _disposedValue;

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(_handle);

            _disposedValue = true;
        }
    }

    ~Shader()
    {
        if (_disposedValue) return;
        Console.WriteLine("GPU Resource leak! Call Dispose()");
        Dispose();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public struct AttribInfo(int loc, int offset, int size)
    {
        public int Location = loc;
        public int Offset = offset;
        public int Size = size;
    }
}