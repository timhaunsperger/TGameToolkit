using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TGameToolkit.Graphics;

public class ComputeShader : IDisposable
{
    public int Handle;
    private readonly Dictionary<string, int> _uniformLocations = new ();

    public ComputeShader(string path)
    {
        // Create Shader
        int computeShader = GL.CreateShader(ShaderType.ComputeShader);
        var src = File.ReadAllText(path);
        GL.ShaderSource(computeShader, src);
        GL.CompileShader(computeShader);
        string infoLogComp = GL.GetShaderInfoLog(computeShader);
        if (infoLogComp != "")
            Console.WriteLine(infoLogComp);
        
        // Create Program
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, computeShader);
        string infoLogProg = GL.GetProgramInfoLog(Handle);
        if (infoLogProg != "")
            Console.WriteLine(infoLogProg);
        GL.LinkProgram(Handle);
        
        GL.DetachShader(Handle, computeShader);
        GL.DeleteShader(computeShader);
        
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            var location = GL.GetUniformLocation(Handle, key);
            _uniformLocations.Add(key, location);
        }
    }
    
    public void Dispatch(int workGroupsX, int workGroupsY, int workGroupsZ)
    {
        GL.UseProgram(Handle);
        GL.DispatchCompute(workGroupsX, workGroupsY, workGroupsZ);
    }

    public void BlockGpu()
    {
        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
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
    
    private bool _disposedValue;
    public void Dispose()
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
            GL.DeleteProgram(Handle);
        }
        GC.SuppressFinalize(this);
    }

    ~ComputeShader()
    {
        if (_disposedValue) return;
        Console.WriteLine("GPU Resource leak! Call Dispose()");
    }
}