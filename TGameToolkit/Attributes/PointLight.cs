using OpenTK.Mathematics;
using TGameToolkit.Drawing;
using TGameToolkit.Lighting;
using TGameToolkit.Objects;

namespace TGameToolkit.Attributes;

public class PointLight : ObjectAttribute
{
    public Vector3 LightColor = (1, 1, 1);
    public float LightStrength = 5f;
    
    public float ConstAtten = 1f;
    public float LinearAtten = 0f;
    public float QuadraticAtten = 1f;
    
    public PointLight()
    {
        Scene.Lights.Add(this);
    }

    public override void Update(double deltaTime) { }
    
    public void Use(Shader shader, int lightNum)
    {
        shader.SetVector3($"pointLights[{lightNum}].color", LightColor);
        shader.SetFloat($"pointLights[{lightNum}].strength", LightStrength);
        
        shader.SetFloat($"pointLights[{lightNum}].constant", ConstAtten);
        shader.SetFloat($"pointLights[{lightNum}].linear", LinearAtten);
        shader.SetFloat($"pointLights[{lightNum}].quadratic", QuadraticAtten);
        
        shader.SetVector3($"pointLights[{lightNum}].pos", Parent.Pos);
    }
}