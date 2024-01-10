using OpenTK.Mathematics;
using TGameToolkit.Attributes;

namespace TGameToolkit.Objects;

public class GameObject
{
    internal static GameObject baseObj = new ();
    
    public Vector3 Pos = Vector3.Zero;
    public List<ObjectAttribute> Attributes = new ();

    public void Update(double deltaTime)
    {
        foreach (var attribute in Attributes)
        {
            attribute.Update(deltaTime);
        }
    }

    public void AttachAttribute(ObjectAttribute attribute)
    {
        Attributes.Add(attribute);
        attribute.Parent = this;
    }
}