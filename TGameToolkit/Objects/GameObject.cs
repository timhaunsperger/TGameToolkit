using OpenTK.Mathematics;
using TGameToolkit.Attributes;
using TGameToolkit.Drawing;

namespace TGameToolkit.Objects;

public class GameObject
{
    internal static readonly GameObject BaseObj = new ();
    
    public Vector3 Pos = Vector3.Zero;
    public List<ObjectAttribute> Attributes = new ();
    public List<RenderMesh> Meshes = new ();

    public void Update(double deltaTime)
    {
        foreach (var attribute in Attributes)
        {
            attribute.Update(deltaTime);
        }
    }
    
    public void RenderMeshes(Shader? shader = null)
    {
        foreach (var mesh in Meshes)
        {
            mesh.Render(shader);
        }
    }
    
    public virtual void OnModify(){}

    public void AttachAttribute(ObjectAttribute attribute)
    {
        Attributes.Add(attribute);
        attribute.Parent = this;
    }
}