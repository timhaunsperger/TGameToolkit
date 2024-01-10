﻿using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TGameToolkit.Drawing;

public class Shader : IDisposable
{
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniformLocations;

    public static readonly Shader UiShader = BuiltIn(
        "TGameToolkit.Shaders.UI.vert", "TGameToolkit.Shaders.UI.frag");
    public static readonly Shader LightingShader = BuiltIn(
        "TGameToolkit.Shaders.basic.vert", "TGameToolkit.Shaders.lighting.frag");

    private static Shader BuiltIn(string vertRsc, string fragRsc)
    {
        var assembly = Assembly.GetExecutingAssembly();
        StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(vertRsc));
        string vertSrc = reader.ReadToEnd();
        reader = new StreamReader(assembly.GetManifestResourceStream(fragRsc));
        string fragSrc = reader.ReadToEnd();
        return new Shader(vertSrc, fragSrc);
    }

    public Shader GenShader(string vertexPath, string fragmentPath)
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
}