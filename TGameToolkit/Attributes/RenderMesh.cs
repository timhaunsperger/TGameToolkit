using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public class RenderMesh : ObjectAttribute
{

    /// <summary>
    /// Format:
    /// X, Y, Z, U, V, nX, nY, nZ 
    /// World coordinates, Tex coordinates, Face Normal
    /// </summary>
    private double[] _vertices;
    
    /// <summary>
    /// Order to connect vertices to form triangles.
    /// </summary>
    private uint[] _indices;

    private int _vao;
    private int _vbo;
    private int _ebo;

    private Vector3d _pos = Vector3d.Zero;

    private Shader _shader;
    private Material _material;
    
    public RenderMesh(Shader shader, Material material, double[] vertices, uint[] indices)
    {
        _shader = shader;
        _material = material;
        _vertices = vertices;
        _indices = indices;
        Init();
    }
    
    /// <summary>
    /// Only call after assigning vertices and indices in derived class
    /// </summary>
    private void Init()
    {
        // Generate Vertex Array Object to store how to draw Element, must be bound first
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        // Generate Vertex Buffer Object to store vertex data
        _vbo = GL.GenBuffer();
        
        // Generate Element Buffer Object to store vertex indices
        _ebo = GL.GenBuffer();
        
        // Set vertex indices
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        // Set vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(double), _vertices, BufferUsageHint.StaticDraw);
        
        // Set attribute pointers to store layout of data in buffers
        var posDataLoc = _shader.GetAttribLocation("aPosition");
        var texDataLoc = _shader.GetAttribLocation("aTexCoord");
        var normDataLoc = _shader.GetAttribLocation("aFaceNorm");

        GL.VertexAttribPointer(
            posDataLoc, 3, VertexAttribPointerType.Double, false, 8 * sizeof(double), 0);
        GL.EnableVertexAttribArray(posDataLoc);
        GL.VertexAttribPointer(
            texDataLoc, 2, VertexAttribPointerType.Double, false, 8 * sizeof(double), 3 * sizeof(double));
        GL.EnableVertexAttribArray(texDataLoc);
        GL.VertexAttribPointer(
            normDataLoc, 3, VertexAttribPointerType.Double, false, 8 * sizeof(double), 5 * sizeof(double));
        GL.EnableVertexAttribArray(normDataLoc);
    }
    
    public override void Update(double deltaTime)
    {
        if (_pos != Parent.Pos)
        {
            var delta = Parent.Pos - _pos;
            for (int i = 0; i < _vertices.Length; i+=8)
            {
                _vertices[i] = delta.X + _vertices[i];
                _vertices[i+1] = delta.Y + _vertices[i+1];
                _vertices[i+2] = delta.Z + _vertices[i+2];
            }
            _pos = Parent.Pos;
        }
        
        Render();
    }
    
    private void Render()
    {
        GL.BindVertexArray(_vao);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertices.Length * sizeof(double), _vertices);
        
        _material.Use(_shader);
        
        _shader.SetMatrix4("view", Scene.GameCamera.GetViewMatrix());
        _shader.SetMatrix4("projection", Scene.GameCamera.GetProjectionMatrix());
        _shader.SetVector3("viewPos", Scene.GameCamera.Pos);
        _shader.SetInt("numLights", Scene.Lights.Count);
        
        for (int i = 0; i < Scene.Lights.Count; i++)
        {
            Scene.Lights[i].Use(_shader, i);
        }
        _shader.Use();

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}

