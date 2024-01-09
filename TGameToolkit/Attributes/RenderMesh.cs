using OpenTK.Graphics.OpenGL4;
using TGameToolkit.Drawing;
using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public abstract class RenderMesh : ObjectAttribute
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

    private Shader _shader;
    private Texture _texture;
    private Camera _camera;
    
    protected RenderMesh(GameObject parent, Camera camera, Shader shader, Texture texture, double[] vertices, uint[] indices) : base(parent)
    {
        _shader = shader;
        _texture = texture;
        _vertices = vertices;
        _indices = indices;
        _camera = camera;
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
        Console.Write(_indices.Length);
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
        Render();
    }
    
    private void Render()
    {
        GL.BindVertexArray(_vao);
        
        //GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        //GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertices.Length * sizeof(double), _vertices);
        
        _texture.Use(TextureUnit.Texture0);
        
        _shader.SetMatrix4("view", _camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        _shader.SetVector3("viewPos", _camera.Pos);
        _shader.Use();

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}