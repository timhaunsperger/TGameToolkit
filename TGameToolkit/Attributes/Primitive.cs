using TGameToolkit.Drawing;
using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public class CubePrimitive(GameObject parent, Camera camera, Shader shader, Texture texture)
    : RenderMesh(parent, camera, shader, texture,
        new []
        {
            // Face 1 - Front
            -1d, -1d, 1d, 0d, 0d, 0d, 0d, 1d,
            1d, -1d, 1d, 1d, 0d, 0d, 0d, 1d,
            1d, 1d, 1d, 1d, 1d, 0d, 0d, 1d,
            -1d, 1d, 1d, 0d, 1d, 0d, 0d, 1d,
            // Face 2 - Back
            1d, -1d, -1d, 0d, 0d, 0d, 0d, -1d,
            -1d, -1d, -1d, 1d, 0d, 0d, 0d, -1d,
            -1d, 1d, -1d, 1d, 1d, 0d, 0d, -1d,
            1d, 1d, -1d, 0d, 1d, 0d, 0d, -1d,
            // Face 3 - Left
            -1d, -1d, 1d, 0d, 0d, -1d, 0d, 0d,
            -1d, -1d, -1d, 1d, 0d, -1d, 0d, 0d,
            -1d, 1d, -1d, 1d, 1d, -1d, 0d, 0d,
            -1d, 1d, 1d, 0d, 1d, -1d, 0d, 0d,
            // Face 4 - Right
            1d, -1d, -1d, 0d, 0d, 1d, 0d, 0d,
            1d, -1d, 1d, 1d, 0d, 1d, 0d, 0d,
            1d, 1d, 1d, 1d, 1d, 1d, 0d, 0d,
            1d, 1d, -1d, 0d, 1d, 1d, 0d, 0d,
            // Face 5 - Top
            -1d, 1d, 1d, 0d, 0d, 0d, 1d, 0d,
            -1d, 1d, -1d, 1d, 0d, 0d, 1d, 0d,
            1d, 1d, -1d, 1d, 1d, 0d, 1d, 0d,
            1d, 1d, 1d, 0d, 1d, 0d, 1d, 0d,
            // Face 6 - Bottom
            -1d, -1d, -1d, 0d, 0d, 0d, -1d, 0d,
            -1d, -1d, 1d, 1d, 0d, 0d, -1d, 0d,
            1d, -1d, 1d, 1d, 1d, 0d, -1d, 0d,
            1d, -1d, -1d, 0d, 1d, 0d, -1d, 0d
        },
        new uint[]
        {
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

