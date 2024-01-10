using OpenTK.Mathematics;

namespace TGameToolkit.Utils;

public static class MathUtils
{
    public static Vector2d GetSphericalCoords(Vector3d pos, Vector3d origin)
    {
        var dir = (pos - origin).Normalized();
        var theta = Math.Atan(dir.Y / dir.X);
        var phi = Math.Acos(dir.Z);

        return (theta, phi);
    }
}