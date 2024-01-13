using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public abstract class ObjectAttribute
{
    public GameObject Parent = GameObject.BaseObj;
    
    public abstract void Update(double deltaTime);
}