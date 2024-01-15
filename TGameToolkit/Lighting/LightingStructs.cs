using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Graphics;
using TGameToolkit.GUI_Elements;

namespace TGameToolkit.Lighting;

public struct DirectionalLight(Vector3 color, float strength, Vector3 dir)
{
    // Color and strength of light
    public Vector3 Color = color;
    public float Strength = strength;
    
    // Light Direction
    public Vector3 Direction = dir;
    
    public void Use(Shader shader)
    {
        shader.SetVector3("directionalLight.color", Color);
        shader.SetFloat("directionalLight.strength", Strength);
        
        shader.SetVector3("directionalLight.direction", Direction);
    }
}