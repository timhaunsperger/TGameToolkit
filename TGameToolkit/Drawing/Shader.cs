using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TGameToolkit.Drawing;

public class Shader
{
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniformLocations;

    public static readonly Shader BasicShader = new Shader(
        "Shaders/UI.vert", "Shaders/UI.frag");

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);
        
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
        _uniformLocations = new Dictionary<string, int>();
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(_handle, i, out _, out _);
            var location = GL.GetUniformLocation(_handle, key);
            _uniformLocations.Add(key, location);
        }
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
    
    private bool _disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(_handle);

            _disposedValue = true;
        }
    }

    ~Shader()
    {
        if (_disposedValue == false)
        {
            Console.WriteLine("GPU Resource leak! Call Dispose()");
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}