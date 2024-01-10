using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;
using TGameToolkit.Utils;

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

    private int _posDataOffset;
    private int _normDataOffset;

    private Vector3d _pos = Vector3d.Zero;

    private Shader _shader;
    private Material _material;

    private bool _centerNormal;
    
    public RenderMesh(Shader shader, Material material, double[] vertices, uint[] indices, bool centerNorm = false)
    {
        _shader = shader;
        _material = material;
        _vertices = vertices;
        _indices = indices;
        _centerNormal = centerNorm;
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
        var stride = _shader.AttribStride;
        foreach (var attrib in _shader.Attributes.Values)
        {
            GL.VertexAttribPointer(
                attrib.Location, attrib.Size, VertexAttribPointerType.Double, false, stride * sizeof(double), attrib.Offset  * sizeof(double));
            GL.EnableVertexAttribArray(attrib.Location);
        }

        _posDataOffset = _shader.Attributes["aPosition"].Offset;
        _normDataOffset = _shader.Attributes["aNormal"].Offset;
    }

    public Vector3d[] GetVertices()
    {
        var vertices = new Vector3d[_vertices.Length / 8];

        for (int i = 0; i < _vertices.Length / 8; i++)
        {
            vertices[i] = new Vector3d(_vertices[i * 8], _vertices[i * 8 + 1], _vertices[i * 8 + 2]);
        }

        return vertices;
    }
    
    public void SetVertices(Vector3d[] vertices, bool updateNormals = true)
    {
        var stride = _shader.AttribStride;
        
        for (int i = 0; i < _vertices.Length / 8; i++)
        {
            var posDataLoc = i * stride + _posDataOffset;
            
            _vertices[posDataLoc] = vertices[i].X;
            _vertices[posDataLoc + 1] = vertices[i].Y;
            _vertices[posDataLoc + 2] = vertices[i].Z;
            
            if (updateNormals && _centerNormal)
            {
                var norm = (vertices[i] - Parent.Pos).Normalized();
                var normDataLoc = i * stride + _normDataOffset;
                
                _vertices[normDataLoc] = norm.X;
                _vertices[normDataLoc + 1] = norm.Y;
                _vertices[normDataLoc + 2] = norm.Z;
            }
        }
        
        if (updateNormals && !_centerNormal)
        {
            for (int i = 0; i < _indices.Length; i += 3)
            {
                // face indices
                var ind0 = _indices[i];
                var ind1 = _indices[i + 1];
                var ind2 = _indices[i + 2];
            
                // face vertex positions
                var v0 = vertices[ind0];
                var v1 = vertices[ind1];
                var v2 = vertices[ind2];
    
                // data offsets
                var o0 = _normDataOffset + ind0 * stride;
                var o1 = _normDataOffset + ind1 * stride;
                var o2 = _normDataOffset + ind2 * stride;

                var norm = Vector3d.Cross(v0 - v1, v2 - v1).Normalized();
                _vertices[o0] = norm.X; _vertices[o0 + 1] = norm.Y; _vertices[o0 + 2] = norm.Z;
                _vertices[o1] = norm.X; _vertices[o1 + 1] = norm.Y; _vertices[o1 + 2] = norm.Z;
                _vertices[o2] = norm.X; _vertices[o2 + 1] = norm.Y; _vertices[o2 + 2] = norm.Z;
            }
        }
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertices.Length * sizeof(double), _vertices);
    }
    
    public override void Update(double deltaTime)
    {
        if (_pos != Parent.Pos)
        {
            var delta = Parent.Pos - _pos;
            for (int i = 0; i < _vertices.Length; i += _shader.AttribStride)
            {
                var posDataLoc = i + _posDataOffset;
                _vertices[posDataLoc] += delta.X;
                _vertices[posDataLoc + 1] += delta.Y;
                _vertices[posDataLoc + 2] += delta.Z;
            }
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertices.Length * sizeof(double), _vertices);
            _pos = Parent.Pos;
        }
        
        Render();
    }
    
    private void Render()
    {
        GL.BindVertexArray(_vao);
        
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

