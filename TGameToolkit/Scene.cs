using TGameToolkit.Attributes;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;

namespace TGameToolkit;

public static class Scene
{
    public static List<GameObject> GameObjects = new ();
    public static Camera GameCamera = new Camera();
    public static List<PointLight> Lights = new ();
}