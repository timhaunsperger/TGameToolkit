using OpenTK.Graphics.OpenGL4;
using TGameToolkit.Drawing;

namespace TGameToolkit.Objects;

public class Postprocessor
{
    private Shader _shader;
    private int _vao;
    private int _vbo;
    
    // Vertices for quad covering whole viewport
    public float[] SceneVertices = {
        -1, 1, 0, 1,   // Top-left
        1, 1, 1, 1,     // Top-Right
        -1, -1, 0, 0, // Bottom-left
        
        1, 1, 1, 1,     // Top-Right
        1, -1, 1, 0,   // Bottom-Right
        -1, -1, 0, 0 // Bottom-left
    };
    // Source for basic vertex shader
    public const string BaseVtxShaderSrc =
        """
        #version 460 core
        layout(location = 0) in vec2 aPosition;
        layout(location = 1) in vec2 aTexCoord;
        out vec2 texCoord;

        void main()
        {
            texCoord = aTexCoord;
            gl_Position = vec4(aPosition, 0.0, 1.0);
        }
        """;
    public const string BaseFragShaderSrc =
        """
        #version 460 core
        out vec4 FragColor;
        in vec2 texCoord;
        
        uniform sampler2D colorTex;
        uniform sampler2D posTex;
        uniform sampler2D depthTex;
        
        void main()
        { 
            FragColor = texture(screenTexture, texCoord);
        }
        """;
    
    public Postprocessor(Shader shader)
    {
        _shader = shader;
        // Generate Vertex Array Object to store how to draw Element, must be bound first
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        // Generate Vertex Buffer Object to store vertex data
        _vbo = GL.GenBuffer();
        
        // Set vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, SceneVertices.Length * sizeof(float), SceneVertices, BufferUsageHint.StaticDraw);
            
        // Set attribute pointers to store layout of data in buffers
        var posDataLoc = _shader.GetAttribLocation("aPosition");
        var texDataLoc = _shader.GetAttribLocation("aTexCoord");
        
        GL.VertexAttribPointer(
            posDataLoc, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(posDataLoc);
        GL.VertexAttribPointer(
            texDataLoc, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        GL.EnableVertexAttribArray(texDataLoc);
    }
    
    public void Use(int frameBuffer)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        GL.BindVertexArray(_vao);
        GL.Disable(EnableCap.DepthTest);

        _shader.Use();
        _shader.SetInt("colorTex", 0);
        _shader.SetInt("posTex", 1);
        _shader.SetInt("depthTex", 2);
        _shader.SetInt("normTex", 3);
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Scene.GeometryPass.ColorTex); 
        
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, Scene.GeometryPass.PosTex); 
        
        GL.ActiveTexture(TextureUnit.Texture2);
        GL.BindTexture(TextureTarget.Texture2D, Scene.GeometryPass.DepthTex); 
        
        GL.ActiveTexture(TextureUnit.Texture3);
        GL.BindTexture(TextureTarget.Texture2D, Scene.GeometryPass.NormalTex); 
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
}