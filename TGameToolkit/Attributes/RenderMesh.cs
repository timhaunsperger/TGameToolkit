using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;
using TGameToolkit.Utils;

namespace TGameToolkit.Attributes;

public class RenderMesh
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

    public Shader Shader { get; }
    public Dictionary<string, Material> Materials = new ();
    
    public RenderMesh(Shader shader, double[] vertices, uint[] indices)
    {
        Shader = shader;
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
        var stride = Shader.AttribStride;
        foreach (var attrib in Shader.Attributes.Values)
        {
            GL.VertexAttribPointer(
                attrib.Location, attrib.Size, VertexAttribPointerType.Double, false, stride * sizeof(double), attrib.Offset  * sizeof(double));
            GL.EnableVertexAttribArray(attrib.Location);
        }

        _posDataOffset = Shader.Attributes["aPosition"].Offset;
        Shader.Attributes.TryGetValue("aNormal", out var normAttrib);
        _normDataOffset = normAttrib.Offset;
    }

    public Vector3d[] GetVertices()
    {
        var vertices = new Vector3d[_vertices.Length / Shader.AttribStride];

        for (int i = 0; i < _vertices.Length / Shader.AttribStride; i++)
        {
            var posDataLoc = _posDataOffset + i * Shader.AttribStride;
            vertices[i] = new Vector3d(_vertices[posDataLoc], _vertices[posDataLoc + 1], _vertices[posDataLoc + 2]);
        }

        return vertices;
    }

    public RenderMesh WithMaterial(string id, Material material)
    {
        Materials[id] = material;
        return this;
    }
    
    public void SetVertices(Vector3d[] vertices, bool updateNormals = true)
    {
        var stride = Shader.AttribStride;
        var vtxPosOffset = Shader.Attributes["aPosition"].Offset;
        for (int i = 0; i < vertices.Length; i++)
        {
            _vertices[vtxPosOffset + i * stride] = vertices[i].X;
            _vertices[vtxPosOffset + i * stride + 1] = vertices[i].Y;
            _vertices[vtxPosOffset + i * stride + 2] = vertices[i].Z;
        }

        if (updateNormals)
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

                var norm = Vector3d.Cross(v1 - v0, v0 - v2).Normalized();
                _vertices[o0] = norm.X; _vertices[o0 + 1] = norm.Y; _vertices[o0 + 2] = norm.Z;
                _vertices[o1] = norm.X; _vertices[o1 + 1] = norm.Y; _vertices[o1 + 2] = norm.Z;
                _vertices[o2] = norm.X; _vertices[o2 + 1] = norm.Y; _vertices[o2 + 2] = norm.Z;
            }
        }
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertices.Length * sizeof(double), _vertices);
    }
    
    public void SetVertexAttribute(string attribName, double[] data, int vertIndex)
    {
        var offset = Shader.Attributes[attribName].Offset + vertIndex * Shader.AttribStride;
        var size = Shader.Attributes[attribName].Size;
        
        for (int i = 0; i < size; i++)
        {
            _vertices[offset + i] = data[i];
        }
    }
    
    public void SetVertexAttribute(string attribName, double data, int vertIndex)
    {
        _vertices[Shader.Attributes[attribName].Offset + vertIndex * Shader.AttribStride] = data;
    }
    
    public virtual void Render(Shader? shader = null)
    {
        GL.BindVertexArray(_vao);

        foreach (var material in Materials)
        {
            material.Value.Use(Shader, material.Key);
        }
        
        Shader.Use();

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}

