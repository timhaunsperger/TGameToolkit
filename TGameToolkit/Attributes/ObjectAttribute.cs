using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public abstract class ObjectAttribute
{
    public GameObject Parent;

    protected ObjectAttribute(GameObject parent)
    {
        Parent = parent;
        parent.AttachAttribute(this);
    }
    
    public abstract void Update(double deltaTime);
}