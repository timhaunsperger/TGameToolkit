using OpenTK.Graphics.OpenGL4;
using TGameToolkit.Drawing;

namespace TGameToolkit.Lighting;

public class Material()
{
    public Texture Tex = Texture.Box((50,100,100,255), 512, 512);
    public float AmbientStrength = 0.5f;
    public float DiffuseStrength = 1f;
    public float SpecularStrength = 2f;
    public float Shininess = 64;

    public void Use(Shader shader, string id)
    {
        Tex.Use(TextureUnit.Texture0);
        shader.SetFloat(id + ".ambient", AmbientStrength);
        shader.SetFloat(id + ".diffuse", DiffuseStrength);
        shader.SetFloat(id + ".specular", SpecularStrength);
        shader.SetFloat(id + ".shininess", Shininess);
    }
}