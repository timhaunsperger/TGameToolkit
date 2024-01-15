using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Utils;

namespace TGameToolkit.Graphics;

public class Shader : IDisposable
{
    public readonly int Handle;
    private readonly Dictionary<string, int> _uniformLocations = new ();
    
    /// <summary>
    /// Stores all vertex attributes, value format: (location (in shader), offset, size)
    /// </summary>
    public readonly Dictionary<string, AttribInfo> Attributes = new ();
    public readonly int AttribStride;

    public static readonly Shader UiShader = BuiltIn(
        "TGameToolkit.Shaders.UI.vert", "TGameToolkit.Shaders.UI.frag", _ => {});

    public Action<Shader> OnUse = _ => {};

    private static Shader BuiltIn(string vertRsc, string fragRsc, Action<Shader> useAction)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(vertRsc));
        string vertSrc = reader.ReadToEnd();
        
        reader = new StreamReader(assembly.GetManifestResourceStream(fragRsc));
        string fragSrc = reader.ReadToEnd();
       
        var shader = new Shader(vertSrc, fragSrc);
        shader.OnUse = useAction;
        
        return shader;
    }

    public static Shader GenShader(string vertexPath, string fragmentPath, Action<Shader>? useAction = null)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);
        
        return new Shader(vertexShaderSource, fragmentShaderSource, useAction);
    }
    
    public Shader(string vertexShaderSource, string fragmentShaderSource, Action<Shader>? useAction = null)
    {
        OnUse = useAction ?? (_ => {});
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
        
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        string infoLogProg = GL.GetProgramInfoLog(Handle);
        if (infoLogProg != "")
            Console.WriteLine(infoLogProg);
        GL.LinkProgram(Handle);
        
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
        
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            var location = GL.GetUniformLocation(Handle, key);
            _uniformLocations.Add(key, location);
        }
        
        GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out var numAttributes);
        var offset = 0;
        
        for (var i = 0; i < numAttributes; i++)
        {
            var key = GL.GetActiveAttrib(Handle, i, out _, out var type);
            var location = GL.GetAttribLocation(Handle, key);
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
        return GL.GetAttribLocation(Handle, attribName);
    }
    
    public void SetInt(string name, int data)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, data);
    }
    
    public void SetFloat(string name, float data)
    {
        GL.UseProgram(Handle);
        GL.Uniform1(_uniformLocations[name], data);
    }
    
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(Handle);
        GL.Uniform3(_uniformLocations[name], data);
    }
    
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }
    
    public void Use()
    {
        OnUse.Invoke(this);
        GL.UseProgram(Handle);
    }
    
    private bool _disposedValue;

    ~Shader()
    {
        if (_disposedValue) return;
        Console.WriteLine("GPU Resource leak! Call Dispose()");
    }
    
    public void Dispose()
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
            GL.DeleteProgram(Handle);
        }
        GC.SuppressFinalize(this);
    }
    
    public struct AttribInfo(int loc, int offset, int size)
    {
        public int Location = loc;
        public int Offset = offset;
        public int Size = size;
    }
}