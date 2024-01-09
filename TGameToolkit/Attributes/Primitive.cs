using OpenTK.Mathematics;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public class CubePrimitive(GameObject parent, Shader shader, Material mat)
    : RenderMesh(parent, shader, mat, 
        new double[] {
            // Face 0.5 - Front
            -0.5d, -0.5d, 0.5d, 0d, 0d, 0d, 0d, 1d,
            0.5d, -0.5d, 0.5d, 0.5d, 0d, 0d, 0d, 1d,
            0.5d, 0.5d, 0.5d, 0.5d, 0.5d, 0d, 0d, 1d,
            -0.5d, 0.5d, 0.5d, 0d, 0.5d, 0d, 0d, 1d,
            // Face 2 - Back
            0.5d, -0.5d, -0.5d, 0d, 0d, 0d, 0d, -1d,
            -0.5d, -0.5d, -0.5d, 0.5d, 0d, 0d, 0d, -1d,
            -0.5d, 0.5d, -0.5d, 0.5d, 0.5d, 0d, 0d, -1d,
            0.5d, 0.5d, -0.5d, 0d, 0.5d, 0d, 0d, -1d,
            // Face 3 - Left
            -0.5d, -0.5d, 0.5d, 0d, 0d, -1d, 0d, 0d,
            -0.5d, -0.5d, -0.5d, 0.5d, 0d, -1d, 0d, 0d,
            -0.5d, 0.5d, -0.5d, 0.5d, 0.5d, -1d, 0d, 0d,
            -0.5d, 0.5d, 0.5d, 0d, 0.5d, -1d, 0d, 0d,
            // Face 4 - Right
            0.5d, -0.5d, -0.5d, 0d, 0d, 1d, 0d, 0d,
            0.5d, -0.5d, 0.5d, 0.5d, 0d, 1d, 0d, 0d,
            0.5d, 0.5d, 0.5d, 0.5d, 0.5d, 1d, 0d, 0d,
            0.5d, 0.5d, -0.5d, 0d, 0.5d, 1d, 0d, 0d,
            // Face 5 - Top
            -0.5d, 0.5d, 0.5d, 0d, 0d, 0d, 1d, 0d,
            -0.5d, 0.5d, -0.5d, 0.5d, 0d, 0d, 1d, 0d,
            0.5d, 0.5d, -0.5d, 0.5d, 0.5d, 0d, 1d, 0d,
            0.5d, 0.5d, 0.5d, 0d, 0.5d, 0d, 1d, 0d,
            // Face 6 - Bottom
            -0.5d, -0.5d, -0.5d, 0d, 0d, 0d, -1d, 0d,
            -0.5d, -0.5d, 0.5d, 0.5d, 0d, 0d, -1d, 0d,
            0.5d, -0.5d, 0.5d, 0.5d, 0.5d, 0d, -1d, 0d,
            0.5d, -0.5d, -0.5d, 0d, 0.5d, 0d, -1d, 0d
        },
        new uint[] {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7,
            8, 9, 10,
            8, 10, 11,
            12, 13, 14,
            12, 14, 15,
            16, 17, 18,
            16, 18, 19,
            20, 21, 22,
            20, 22, 23
        });
