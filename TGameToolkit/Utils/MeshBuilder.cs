using System.Runtime.InteropServices.JavaScript;
using OpenTK.Mathematics;
using TGameToolkit.Attributes;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;

namespace TGameToolkit.Utils;

public static class MeshBuilder
{
    public static RenderMesh GetCubeMesh(
        float scale = 1, Quaterniond? rotation = null, Material? material = null, Shader? shader = null)
    {
        Vector3d[] vertices =
        {
            // Face 1 - Front
            (-0.5d, -0.5d, 0.5d), (0.5d, -0.5d, 0.5d), (0.5d, 0.5d, 0.5d), (-0.5d, 0.5d, 0.5d),
            // Face 2 - Back
            (0.5d, -0.5d, -0.5d), (-0.5d, -0.5d, -0.5d), (-0.5d, 0.5d, -0.5d), (0.5d, 0.5d, -0.5d),
            // Face 3 - Left
            (-0.5d, -0.5d, 0.5d), (-0.5d, -0.5d, -0.5d), (-0.5d, 0.5d, -0.5d), (-0.5d, 0.5d, 0.5d),
            // Face 4 - Right
            (0.5d, -0.5d, -0.5d), (0.5d, -0.5d, 0.5d), (0.5d, 0.5d, 0.5d), (0.5d, 0.5d, -0.5d),
            // Face 5 - Top
            (-0.5d, 0.5d, 0.5d), (-0.5d, 0.5d, -0.5d), (0.5d, 0.5d, -0.5d), (0.5d, 0.5d, 0.5d),
            // Face 6 - Bottom
            (-0.5d, -0.5d, -0.5d), (-0.5d, -0.5d, 0.5d), (0.5d, -0.5d, 0.5d), (0.5d, -0.5d, -0.5d)
        };
        Vector2d[] texCoords =
        {
            (0d, 0d), (0.5d, 0d), (0.5d, 0.5d), (0d, 0.5d),
            (0d, 0d), (0.5d, 0d), (0.5d, 0.5d), (0d, 0.5d),
            (0d, 0d), (0.5d, 0d), (0.5d, 0.5d), (0d, 0.5d),
            (0d, 0d), (0.5d, 0d), (0.5d, 0.5d), (0d, 0.5d), 
            (0d, 0d), (0.5d, 0d), (0.5d, 0.5d), (0d, 0.5d), 
            (0d, 0d), (0.5d, 0d), (0.5d, 0.5d), (0d, 0.5d)
        };
        uint[] indices = {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };

        var vertexData = 
            GetVertexData(vertices, texCoords, scale, rotation ?? Quaterniond.Identity, indices, shader ?? Shader.LightingShader);

        return new RenderMesh(shader ?? Shader.LightingShader, material ?? new Material(), vertexData, indices);

    }

    private static (Vector3d[], Vector2d[], uint[]) PlaneData(int res, Vector3d? planeNormal, bool onUnitCube = false)
    {
        Vector3d[] vertices = new Vector3d[res * res];
        Vector2d[] texCoords = new Vector2d[res * res];
        uint[] indices = new uint[(res - 1) * (res - 1) * 6];
       
        var norm = (planeNormal ?? Vector3d.UnitY).Normalized();
        
        Vector3d axisA = norm.Yzx;
        Vector3d axisB = Vector3d.Cross(norm, norm.Yzx);
        
        var triIndex = 0;
        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                var i = (uint)(y * res + x);
                var planePos = new Vector2d(x, y) / (res - 1) * 2 - Vector2d.One;
                vertices[i] = axisA * planePos.X + axisB * planePos.Y + norm * (onUnitCube ? 1 : 0);
                texCoords[i] = (planePos + Vector2d.One) / 2;
                
                if (x != res - 1 && y != res - 1)
                {
                    indices[triIndex] = i;
                    indices[triIndex + 1] = i + (uint)res;
                    indices[triIndex + 2] = i + (uint)res + 1;
                    indices[triIndex + 3] = i;
                    indices[triIndex + 4] = i + (uint)res + 1;
                    indices[triIndex + 5] = i + 1;

                    triIndex += 6;
                }
            }
        }
        
        
        return (vertices, texCoords, indices);
    }

    public static RenderMesh GetPlaneMesh(int resolution = 4, float scale = 1, Vector3? normal = null, Material? mat = null, Shader? shader = null)
    {
        var data = PlaneData(resolution, normal);
        var vertData = GetVertexData(data.Item1, data.Item2, scale, Quaterniond.Identity, data.Item3, shader ?? Shader.LightingShader);
        return new RenderMesh(shader ?? Shader.LightingShader, mat ?? new Material(), vertData, data.Item3);
    }
    
    public static RenderMesh GetNcSphereMesh(
        int resolution = 2, float scale = 1, Quaterniond? rotation = null, Material? material = null, Shader? shader = null)
    {
        var faceVertNum = resolution * resolution;
        var faceIndices = (resolution - 1) * (resolution - 1) * 6;
        
        var vertices = new Vector3d[faceVertNum * 6];
        var texCoords = new Vector2d[faceVertNum * 6];
        var indices = new uint[faceIndices * 6];

        Vector3d[] cardinals = { Vector3d.UnitZ, -Vector3d.UnitZ, Vector3d.UnitX, -Vector3d.UnitX, Vector3d.UnitY, -Vector3d.UnitY };
        for (int i = 0; i < 6; i++)
        {
            var meshData = PlaneData(resolution, cardinals[i], true);
            Array.Copy(meshData.Item1, 0, vertices, faceVertNum * i, faceVertNum);
            Array.Copy(meshData.Item2, 0, texCoords, faceVertNum * i, faceVertNum);
            for (int j = 0; j < meshData.Item3.Length; j++)
            {
                indices[faceIndices * i + j] = meshData.Item3[j] + (uint)(faceVertNum * i);
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i].Normalized();
        }

        var vertexData = GetVertexData(vertices, texCoords, scale, rotation ?? Quaterniond.Identity, Vector3d.Zero, shader ?? Shader.LightingShader);
        
        return new RenderMesh(Shader.LightingShader, material ?? new Material(), vertexData, indices, true);

    }
    
    /// <summary>
    /// Construct vertex data array using vertex normals
    /// </summary>
    public static double[] GetVertexData(Vector3d[] baseVertices, Vector2d[] texCoords, float scale, Quaterniond rotation, Vector3d center, Shader shader)
    {
        var output = new double[baseVertices.Length * 8];

        var posOffset = shader.Attributes["aPosition"].Offset;
        var texOffset = shader.Attributes["aTexCoord"].Offset;
        var normOffset = shader.Attributes["aNormal"].Offset;
        
        for (int i = 0; i < baseVertices.Length; i++)
        {
            var vert = rotation * baseVertices[i] * scale;
            var norm = (vert - center).Normalized();
            
            output[i * 8 + posOffset] = vert.X;
            output[i * 8 + posOffset + 1] = vert.Y;
            output[i * 8 + posOffset + 2] = vert.Z;
            output[i * 8 + texOffset] = Math.Clamp(texCoords[i].X, 0.01, 0.99);
            output[i * 8 + texOffset + 1] = Math.Clamp(texCoords[i].Y, 0.01, 0.99);
            output[i * 8 + normOffset] = norm.X;
            output[i * 8 + normOffset + 1] = norm.Y;
            output[i * 8 + normOffset + 2] = norm.Z;
        }
        return output;
    }
    
    /// <summary>
    /// Construct vertex data array using face normals
    /// </summary>
    public static double[] GetVertexData(
        Vector3d[] baseVertices, Vector2d[] texCoords, float scale, Quaterniond rotation, uint[] indices, 
        Shader shader)
    {
        var output = new double[baseVertices.Length * 8];
        
        var posOffset = shader.Attributes["aPosition"].Offset;
        var texOffset = shader.Attributes["aTexCoord"].Offset;
        var normOffset = shader.Attributes["aNormal"].Offset;
        
        for (int i = 0; i < baseVertices.Length; i++)
        {
            var vert = rotation * baseVertices[i] * scale;
            output[i * 8 + posOffset] = vert.X;
            output[i * 8 + posOffset + 1] = vert.Y;
            output[i * 8 + posOffset + 2] = vert.Z;
            output[i * 8 + texOffset] = Math.Clamp(texCoords[i].X, 0.01, 0.99);
            output[i * 8 + texOffset + 1] = Math.Clamp(texCoords[i].Y, 0.01, 0.99);
        }

        for (int i = 0; i < indices.Length; i += 3)
        {
            var ind0 = indices[i];
            var ind1 = indices[i + 1];
            var ind2 = indices[i + 2];
            
            var v0 = baseVertices[ind0];
            var v1 = baseVertices[ind1];
            var v2 = baseVertices[ind2];
            
            var norm = Vector3d.Cross(v0 - v1, v2 - v1).Normalized();

            output[ind0 * 8 + normOffset] = norm.X; 
            output[ind0 * 8 + normOffset + 1] = norm.Y; 
            output[ind0 * 8 + normOffset + 2] = norm.Z;
            
            output[ind1 * 8 + normOffset] = norm.X; 
            output[ind1 * 8 + normOffset + 1] = norm.Y; 
            output[ind1 * 8 + normOffset + 2] = norm.Z;
            
            output[ind2 * 8 + normOffset] = norm.X; 
            output[ind2 * 8 + normOffset + 1] = norm.Y; 
            output[ind2 * 8 + normOffset + 2] = norm.Z;
        }

        return output;
    }
}


