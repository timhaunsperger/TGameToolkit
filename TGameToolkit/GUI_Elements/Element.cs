using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TGameToolkit.Drawing;
using TGameToolkit.Windowing;

namespace TGameToolkit.GUI_Elements;

/// <summary>
/// Class to represent a single drawn GUI element.
/// </summary>
public class Element
{
    // Element data
    public readonly GuiWindow Window;
    private Vector2i _pos;
    private Vector2i _size;
    public Vector2 AnchorPoint;
    public Vector2i Size => _size;
    public Vector2i Pos => _pos;
    
    /// <summary>
    /// The vector from the upper-left corner of the element's bounding box to _pos.
    /// </summary>
    public Vector2i PosOffset => _pos - BoundingBox.Min;
    
    public Box2i BoundingBox { get; private set; }
    public AlignMode Align;
    public HashSet<string> Flags = new();

    // Basic interaction actions
    public Action<Element, Vector2i, MouseButtonEventArgs> OnMouseClick = delegate {  };
    public Action<Element, Vector2i, MouseButtonEventArgs> OnMouseUp = delegate {  };
    public Action<Element, string> OnTextInput = delegate {  };
    public Action<Element, KeyboardKeyEventArgs> OnKeyInput = delegate {  };
    public Action<Element> OnDraw = delegate {  };
    internal Action<Element, MouseMoveEventArgs, MouseState> OnMouseMove =
        delegate(Element e, MouseMoveEventArgs m, MouseState mouseState)
        {
            var isWithin = e.BoundingBox.ContainsInclusive((Vector2i)m.Position);
            var wasWithin = e.BoundingBox.ContainsInclusive((Vector2i)(m.Position - m.Delta));
            
            if (mouseState.IsButtonDown(MouseButton.Button1))
            {
                e.OnMouseDrag.Invoke(e, m);
            }
            if (isWithin && !wasWithin) e.OnMouseEnter.Invoke(e);
            if (wasWithin && !isWithin) e.OnMouseExit.Invoke(e);
        };
    
    // Derived interaction actions
    public Action<Element> OnMouseEnter = delegate {  };
    public Action<Element> OnMouseExit = delegate {  };
    public Action<Element, MouseMoveEventArgs> OnMouseDrag = delegate {  };
    
    // Related GUI elements
    public Element? Parent { get; private set; }
    public Dictionary<string, Element> Children = new();

    // Order to connect vertices to draw triangles
    private readonly uint[] _indices = {
        0, 1, 2,
        0, 2, 3
    };
    
    // Shader and textures for GUI element
    private Shader _shader;
    public Texture Tex { get; private set; }

    // Vertex texture coordinates
    private float _texMinX;
    private float _texMaxX = 1;
    private float _texMinY;
    private float _texMaxY = 1;
    
    // OpenGL object ids for drawing GUI Element.
    private int _vao; // Stores data layout in buffers.
    private int _vbo; // Stores vertex data.
    private int _ebo; // Stores index data to connect vertices.

    /// <summary>
    /// Creates a new GUI Element.
    /// </summary>
    /// <param name="window">GUI Window this element is drawn in.</param>
    /// <param name="pos">Position of element in normalized window coordinates relative to parent, or relative to center of window if element is root.</param>
    /// <param name="shader">Shader program to use when rendering element.</param>
    /// <param name="texture">Tex to use when rendering element.</param>
    /// <param name="align">Which point on the element will correspond to its position.</param>
    /// <param name="size">Vector representing size of GUI element bounding box.</param>
    /// <param name="anchorPoint">Position in normalized window space to anchor element, only used if element is root.</param>
    public Element(
        GuiWindow window,
        Vector2i? pos = null,
        Shader? shader = null,
        Texture? texture = null,
        AlignMode align = AlignMode.Default,
        Vector2i? size = null,
        Vector2? anchorPoint = null)
    {
        Window = window;
        Align = align;
        _pos = pos ?? (0, 0);
        _shader = shader ?? Shader.UiShader;
        Tex = texture ?? Texture.Blank;
        _size = size ?? new Vector2i(Tex.Width, Tex.Height);
        AnchorPoint = anchorPoint ?? new Vector2(0, 0);
        if (size != null) { Flags.Add("FixedSize"); }
        Init();
    }
    
    private void Init()
    {
        // Generate Vertex Array Object to store how to draw Element, must be bound first
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        // Generate Vertex Buffer Object to store vertex data
        _vbo = GL.GenBuffer();
        
        // Generate Element Buffer Object to store vertex indices
        _ebo = GL.GenBuffer();
        
        // Set vertex indices
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        
        // Set vertex data
        UpdateVertices();
        
        // Set attribute pointers to store layout of data in buffers
        var posDataLoc = _shader.GetAttribLocation("aPosition");
        var texDataLoc = _shader.GetAttribLocation("aTexCoord");
        
        GL.VertexAttribPointer(
            posDataLoc, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.EnableVertexAttribArray(posDataLoc);
        GL.VertexAttribPointer(
            texDataLoc, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        GL.EnableVertexAttribArray(texDataLoc);
        
        // Activate texture
        UpdateTexture(Tex);
    }
    

    /// <summary>
    /// Adds a child to the element which will be drawn relative to the position of this element.
    /// </summary>
    /// <param name="name">Key of element in internal dictionary of children.</param>
    /// <param name="element">Element to add as child.</param>
    public void AddChild(string name, Element element)
    {
        element.Parent = this;
        element.UpdateVertices();
        Children[name] = element;
    }
    
    public List<Element> GetChildrenByFlag(string flag)
    {
        var flaggedChildren = new List<Element>();
        if (Flags.Contains(flag))
        {
            flaggedChildren.Add(this);
        }

        foreach (var child in Children.Values)
        {
            flaggedChildren.AddRange(child.GetChildrenByFlag(flag));
        }

        return flaggedChildren;
    }

    /// <summary>
    /// Gets the offset from the minimum y position to the center y position.
    /// </summary>
    /// <returns></returns>
    public int GetCenterYOffset()
    {
        return (int)BoundingBox.Center.Y - BoundingBox.Min.Y;
    }

    
    /// <summary>
    /// Draw GUI element and all children.
    /// </summary>
    public void Draw()
    {
        OnDraw.Invoke(this);
        if (!Flags.Contains("Invisible"))
        {
            Render();
        }

        foreach (var child in Children.Values)
        {
            child.Draw();
        }
    }

    internal void Render()
    {
        // Recall VAO with object data
        GL.BindVertexArray(_vao);
        
        // Activate textures
        Tex.Use(TextureUnit.Texture0);
        
        _shader.Use();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
    
    /// <summary>
    /// Update element texture
    /// </summary>
    /// <param name="newTex">Set new texture</param>
    public void UpdateTexture(Texture newTex)
    {
        if (Tex == newTex) { return; }
        
        // Must activate shader to set texture units 
        _shader.Use();
        
        _shader.SetInt("texture", _shader.GetAttribLocation("texture"));

        if (!Flags.Contains("FixedSize"))
        {
            _size = (newTex.Width, newTex.Height);
            UpdateVertices();
        }
        
        Tex = newTex;
    }
    
    private Vector2i GetAbsPos()
    {
        return Parent == null ? Pos + (Vector2i)(AnchorPoint * Window.ClientSize) : Parent.GetAbsPos() + Pos;
    }
    
    /// <summary>
    /// Gets vertices of element in pixel units.
    /// </summary>
    /// <returns>Array of vectors beginning with bottom-left and continuing counter-clockwise.</returns>
    private Vector2i[] GetPixelVertices()
    {
        var absPos = GetAbsPos();
        var size = Size;
        Vector2i[] vertices;
        
        // Min and max y flipped since opengl uses bottom y origin and GUI uses top y origin
        switch (Align)
        {
            case AlignMode.UpperLeft:
                vertices = new[]
                {   //           X Pos                Y Pos
                    new Vector2i(absPos.X,            absPos.Y + size.Y), 
                    new Vector2i(absPos.X + size.X, absPos.Y + size.Y), 
                    new Vector2i(absPos.X + size.X, absPos.Y), 
                    new Vector2i(absPos.X,            absPos.Y)
                };
                break;
            case AlignMode.CenterLeft:
                vertices = new[]
                {   //           X Pos                Y Pos
                    new Vector2i(absPos.X,            absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X + size.X, absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X + size.X, absPos.Y - size.Y / 2), 
                    new Vector2i(absPos.X,            absPos.Y - size.Y / 2)
                };
                break;
            case AlignMode.LowerLeft:
                vertices = new[]
                {   //           X Pos                Y Pos
                    new Vector2i(absPos.X,            absPos.Y), 
                    new Vector2i(absPos.X + size.X, absPos.Y), 
                    new Vector2i(absPos.X + size.X, absPos.Y - size.Y), 
                    new Vector2i(absPos.X,            absPos.Y - size.Y)
                };
                break;
            case AlignMode.UpperCenter:
                vertices = new[]
                {   //           X Pos                   Y Pos
                    new Vector2i(absPos.X - size.X / 2, absPos.Y + size.Y), 
                    new Vector2i(absPos.X + size.X / 2, absPos.Y + size.Y), 
                    new Vector2i(absPos.X + size.X / 2, absPos.Y),
                    new Vector2i(absPos.X - size.X / 2, absPos.Y)
                };
                break;
            case AlignMode.Center:
                vertices = new[]
                {   //           X Pos                   Y Pos
                    new Vector2i(absPos.X - size.X / 2, absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X + size.X / 2, absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X + size.X / 2, absPos.Y - size.Y / 2),
                    new Vector2i(absPos.X - size.X / 2, absPos.Y - size.Y / 2)
                };
                break;
            case AlignMode.LowerCenter:
                vertices = new[]
                {   //           X Pos                   Y Pos
                    new Vector2i(absPos.X - size.X / 2, absPos.Y), 
                    new Vector2i(absPos.X + size.X / 2, absPos.Y), 
                    new Vector2i(absPos.X + size.X / 2, absPos.Y - size.Y),
                    new Vector2i(absPos.X - size.X / 2, absPos.Y - size.Y)
                };
                break;
            
            case AlignMode.UpperRight:
                vertices = new[]
                {   //           X Pos                   Y Pos
                    new Vector2i(absPos.X - size.X,  absPos.Y + size.Y), 
                    new Vector2i(absPos.X,             absPos.Y + size.Y), 
                    new Vector2i(absPos.X,             absPos.Y), 
                    new Vector2i(absPos.X - size.X,  absPos.Y)
                };
                break;
            
            case AlignMode.CenterRight:
                vertices = new[]
                {   //           X Pos                   Y Pos
                    new Vector2i(absPos.X - size.X,  absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X,             absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X,             absPos.Y - size.Y / 2), 
                    new Vector2i(absPos.X - size.X,  absPos.Y - size.Y / 2)
                };
                break;
            case AlignMode.LowerRight:
                vertices = new[]
                {   //           X Pos                   Y Pos
                    new Vector2i(absPos.X - size.X,  absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X,             absPos.Y + size.Y / 2), 
                    new Vector2i(absPos.X,             absPos.Y - size.Y / 2), 
                    new Vector2i(absPos.X - size.X,  absPos.Y - size.Y / 2)
                };
                break;
            default:
                throw new Exception("Invalid Align Argument");
        }

        return vertices;
    }
    
    public void UpdateVertices()
    {
        var xScale = Window.ClientSize.X / 2f;
        var yScale = Window.ClientSize.Y / 2f;
        var pixVertices = GetPixelVertices();
        BoundingBox = new Box2i(pixVertices[0], pixVertices[2]);
        
        float[] vertices =
        {
            // X cord                    Y Cord                       Tex cords [x,y]
            pixVertices[0].X/xScale - 1, 1 - pixVertices[0].Y/yScale, _texMinX, _texMinY,
            pixVertices[1].X/xScale - 1, 1 - pixVertices[1].Y/yScale, _texMaxX, _texMinY,
            pixVertices[2].X/xScale - 1, 1 - pixVertices[2].Y/yScale, _texMaxX, _texMaxY,
            pixVertices[3].X/xScale - 1, 1 - pixVertices[3].Y/yScale, _texMinX, _texMaxY
        };

        // Set vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        foreach (var child in Children.Values)
        {
            child.UpdateVertices();
        }
    }
    
    /// <summary>
    /// Sets relative position of element.
    /// </summary>
    /// <param name="newPos">Coordinates in pixels.</param>
    public void SetPos(Vector2i newPos)
    {
        _pos = newPos;
        UpdateVertices();
    }
    
    /// <summary>
    /// Moves position of element by vector.
    /// </summary>
    /// <param name="offset">Vector in pixels.</param>
    public void Move(Vector2i offset)
    {
        _pos += offset;
        UpdateVertices();
    }

    public void Resize(int x, int y)
    {
        _size.X = x;
        _size.Y = y;
        UpdateVertices();
    }
    
    /// <summary>
    /// Sets section of texture drawn on element.
    /// </summary>
    /// <param name="xMin">Minimum X coordinate drawn as fraction of total width, range:0-1</param>
    /// <param name="xMax">Maximum X coordinate drawn as fraction of total width, range:0-1</param>
    /// <param name="yMin">Minimum Y coordinate drawn as fraction of total height, range:0-1</param>
    /// <param name="yMax">Maximum Y coordinate drawn as fraction of total height, range:0-1</param>
    public void SetTexCoords(float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f)
    {
        _texMinX = xMin;
        _texMaxX = xMax;
        _texMinY = yMin;
        _texMaxY = yMax;
        UpdateVertices();
    }

    private bool _disposed;
    protected void Dispose()
    {
        if (Window.RootElements.Contains(this) && !_disposed)
        {
            Window.DisposedElements.Add(this);
            _disposed = true;
        }
        else
        {
            throw new InvalidOperationException(
                "Dispose method only used for root elements, instead use Children.Remove() on parent object");
        }
    }
}

public enum AlignMode
{
    Default = 0,
    UpperLeft = 0,
    CenterLeft = 1,
    LowerLeft = 2,
    UpperCenter = 3,
    Center = 4,
    LowerCenter = 5,
    UpperRight = 6,
    CenterRight = 7,
    LowerRight = 8,
}