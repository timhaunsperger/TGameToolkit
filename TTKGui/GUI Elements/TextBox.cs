using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TTKGui.Text;
using TTKGui.Windowing;

namespace TTKGui.GUI_Elements;

public class TextBox : Element
{
    public string Text = "";
    protected int[] CharWidths = Array.Empty<int>();
    private string _prevText = "";
    private LinkedList<(string, int, int, bool)> _textHistory = new ();
    private readonly int _capacity;
    
    public Vector4i TextColor;
    private readonly int _textSize;
    
    private int _cursorFlashClock;
    protected int CursorLoc;
    protected int HighlightAnchor;
    

    protected int BoxWidth;
    protected int Padding = 5;
    private int _textOffset;
    private int _netTextWidth;
    
    protected readonly Element TextWriter, Cursor; 
    protected readonly Texture CursorTexStandard, CursorTexHighlight;

    private static string _breakChars = "!?,.\"$%#@&()~-+*^/(){}[]|\\ \n";

    public Action<Element, string> OnTextSubmit = (e, s) => { };
    public Action<Element, string> OnTextUpdate = (e, s) => { };
    public TextBox(GuiWindow window, Vector2i pos, Vector2i size, AlignMode align = AlignMode.Default, 
        Texture? boxTex = null, Vector4i? textColor = null, string defaultText = "", int textSize = 0, int capacity = 1000) 
        : base(window, pos, Shader.BasicShader, boxTex ?? Texture.Box(Theme.Background, size), align, size)
    {
        // Set Actions
        OnMouseClick = ClickAction;
        OnTextInput = TextInpAction;
        OnKeyInput = KeyInpAction;
        OnMouseDrag = MouseDragAction;
        OnMouseUp = MouseUpAction;
        OnDraw = DrawAction;
        
        // Set properties
        TextColor = textColor ?? Theme.Text;
        _textSize = textSize == 0 ? size.Y / 2 : textSize;
        BoxWidth = Size.X - Padding * 2;
        _capacity = capacity;
        
        // Generate text writer element
        var defaultTex = TextGenerator.GetStringTex(defaultText, _textSize, TextColor);
        
        TextWriter = new Element(
            window, new Vector2i(BoundingBox.Min.X - Pos.X + Padding, (int)BoundingBox.Center.Y - Pos.Y), 
            Shader.BasicShader, defaultTex, AlignMode.CenterLeft, (BoxWidth, defaultTex.Height));
        TextWriter.SetTexCoords(0, BoxWidth / (float)defaultTex.Width);

        // Generate cursor elements and textures
        CursorTexStandard = Texture.Box(TextColor, int.Max(textSize / 14, 1), (int)(_textSize * 1.2));
        CursorTexHighlight = Texture.Box((50, 50, 255, 100), BoxWidth, (int)(_textSize * 1.2));
        Cursor = new Element(window, new Vector2i(0, 0), Shader.BasicShader, CursorTexStandard, AlignMode.CenterLeft);
        AddChild("textWriter", TextWriter);
    }

    private void ClickAction(Element e, Vector2i mousePos, MouseButtonEventArgs args)
    {
        if (args.Button != MouseButton.Button1) return;
        if (BoundingBox.ContainsInclusive(mousePos))
        {
            Flags.Add("Active");
            TextWriter.AddChild("cursor", Cursor);
            
            AlignPosToText(mousePos, out var mouseLoc);
            Console.WriteLine(mouseLoc);
            SetCursor(mouseLoc);
        }
        else if (Flags.Contains("Active"))
        {
            Flags.Remove("Active");
            TextWriter.Children.Remove("cursor");
            Cursor.Flags.Remove("Invisible");
        }
    }

    private void MouseUpAction(Element e, Vector2i mousePos, MouseButtonEventArgs args)
    {
        
    }
    
    private void MouseDragAction(Element e, MouseMoveEventArgs args)
    {
        if (!Flags.Contains("Active")) return;
        AlignPosToText((Vector2i)args.Position, out var mouseLoc);
        if (mouseLoc == CursorLoc) return;
        SetCursor(mouseLoc, true);
    }

    private void DrawAction(Element e)
    {
        if (Flags.Contains("HighlightText")) return;
        
        _cursorFlashClock += 1;
        if (_cursorFlashClock == (int)e.Window.UpdateFrequency / 2)
        {
            Cursor.Flags.Add("Invisible");
        }
        if (_cursorFlashClock >= (int)e.Window.UpdateFrequency)
        {
            Cursor.Flags.Remove("Invisible");
            _cursorFlashClock = 0;
        }
    }

    private void TextInpAction(Element e, string s)
    {
        if (!Flags.Contains("Active")) return;
        Insert(s);
    }

    private void KeyInpAction(Element e, KeyboardKeyEventArgs k)
    {
        if (!Flags.Contains("Active")) return;
        var highlight = (k.Modifiers & KeyModifiers.Shift) != 0;
        switch (k.Key)
        {
            case Keys.Backspace:
            {
                Remove();
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    var removeCount = 0;
                    while (CursorLoc - removeCount > 0 && !_breakChars.Contains(Text[CursorLoc - 1 - removeCount]))
                    {
                        removeCount++;
                    }
                    Remove(removeCount);
                }
                break;
            }
            case Keys.Delete:
            {
                if (CursorLoc == Text.Length) return;
                CursorLoc += 1;
                Remove();
                
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    var removeCount = 0;
                    while (CursorLoc + removeCount < Text.Length && !_breakChars.Contains(Text[CursorLoc + removeCount]))
                    {
                        removeCount++;
                    }
                    CursorLoc += removeCount;
                    Remove(removeCount);
                }
                break;
            }
            case Keys.Right:
                MoveCursor(1, highlight);
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    while (CursorLoc < Text.Length && !_breakChars.Contains(Text[CursorLoc]))
                    {
                        MoveCursor(1, highlight);
                    }
                }
                break;
            case Keys.Left:
                MoveCursor(-1, highlight);
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    while (CursorLoc > 0 && !_breakChars.Contains(Text[CursorLoc - 1]))
                    {
                        MoveCursor(-1, highlight);
                    }
                }
                break;
            case Keys.Home:
                SetCursor(0, highlight);
                break;
            case Keys.End:
                SetCursor(Text.Length, highlight);
                break;
            case Keys.Enter:
                OnTextSubmit.Invoke(this, Text);
                Flags.Remove("Active");
                TextWriter.Children.Remove("cursor");
                Cursor.Flags.Remove("Invisible");
                break;
            case Keys.X:
                if ((k.Modifiers & KeyModifiers.Control) != 0 && Flags.Contains("HighlightText"))
                {
                    Copy();
                    Remove();
                }
                break;
            case Keys.C:
                if ((k.Modifiers & KeyModifiers.Control) != 0 && Flags.Contains("HighlightText"))
                {
                    Copy();
                }
                break;
            case Keys.V:
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    Paste();
                }
                break;
            case Keys.A:
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    HighlightAnchor = 0;
                    SetCursor(Text.Length, true);
                }
                break;
            case Keys.Z:
                if ((k.Modifiers & KeyModifiers.Control) != 0)
                {
                    var prevData = _textHistory.Last?.Value;
                    if (prevData == null) return;
                    
                    Text = prevData.Value.Item1;
                    CursorLoc = prevData.Value.Item2;
                    
                    HighlightAnchor = prevData.Value.Item3;
                    if (_textHistory.Count > 1)
                    {
                        _textHistory.RemoveLast();
                    }
                    UpdateTextBox(prevData.Value.Item4);
                }
                break;
        }
    }

    private void AlignPosToText(Vector2i position, out int charIndexBefore)
    {
        var widthCounter = -_textOffset;
        position.X -= TextWriter.BoundingBox.Min.X;
        position.Y -= TextWriter.BoundingBox.Min.Y;
        charIndexBefore = Text.Length;
        
        
        for (var i = 0; i < Text.Length; i++)
        {
            if (position.X <= widthCounter + CharWidths[i] / 2)
            {
                charIndexBefore = i;
                break;
            }
            widthCounter += CharWidths[i];
        }
    }

    private void Insert(string str)
    {
        if (_textHistory.Count > 100)
        {
            _textHistory.RemoveFirst();
        }
        _textHistory.AddLast((Text, CursorLoc, HighlightAnchor, false));
        
        if (Flags.Contains("HighlightText")) Remove();
        if (Text.Length + str.Length > _capacity) return;
        
        Text = Text.Insert(CursorLoc, str);
        CursorLoc += str.Length;
        
        UpdateTextBox();
        OnTextUpdate.Invoke(this, Text);
    }

    private void Remove(int count = 1)
    {
        if (_textHistory.Count > 100)
        {
            _textHistory.RemoveFirst();
        }
        _textHistory.AddLast((Text, CursorLoc, HighlightAnchor, Flags.Contains("HighlightText")));
        
        int removedWidth;
        if (Flags.Contains("HighlightText"))
        {
            var range = GetHighlightRange();
            CursorLoc = range.Start.Value;
            Text = Text.Remove(CursorLoc, range.End.Value - range.Start.Value);
            removedWidth = CharWidths[range].Sum();
        }
        else 
        {
            if(CursorLoc == 0) return;
            CursorLoc -= count;
            Text = Text.Remove(CursorLoc, count);
            removedWidth = CharWidths[CursorLoc .. (CursorLoc + count)].Sum();
        }

        _textOffset -= removedWidth;
        if (_textOffset < 0)
        {
            _textOffset = 0;
        }
        
        UpdateTextBox();
        OnTextUpdate.Invoke(this, Text);
    }
    
    private void MoveCursor(int delta, bool highlight = false)
    {
        var newPos = CursorLoc + delta;
        
        if (Flags.Contains("HighlightText") && !highlight)
        {
            var range = GetHighlightRange();
            newPos = delta > 0 ? range.End.Value : range.Start.Value ;
        }
        SetCursor(newPos, highlight);
    }
    
    private void SetCursor(int newLoc, bool highlight = false)
    {
        if (newLoc < 0) newLoc = 0;
        if (newLoc > Text.Length) newLoc = Text.Length;
        CursorLoc = newLoc;
        
        if (highlight)
        {
            if (HighlightAnchor != CursorLoc)
            {
                UpdateTextBox(true);
                return;
            }
        }
        UpdateTextBox();
        
    }

    private void Copy()
    {
        Clipboard.SetClipboard(Window, Text[GetHighlightRange()]);
    }
    
    private void Paste()
    {
        Insert(Clipboard.GetClipboard(Window));
    }

    private Range GetHighlightRange()
    {
        return Math.Min(CursorLoc, HighlightAnchor) .. Math.Max(HighlightAnchor, CursorLoc);
    }
    
    private void UpdateTextBox(bool keepHighlight = false)
    {
        if (Text != _prevText)
        {
            _prevText = Text;
            
            // Update Texture
            TextGenerator.GetStringData(
                Text,
                _textSize,
                TextColor,
                out CharWidths,
                out byte[] texData,
                out _netTextWidth,
                out int height);
            
            // Ensure texture has valid size
            if (_netTextWidth == 0)
            {
                TextWriter.UpdateTexture(Texture.Blank);
            }
            else if (_netTextWidth < 16384)
            {
                TextWriter.UpdateTexture(new Texture(texData, _netTextWidth, height));
            }
        }
        
        AlignText(keepHighlight, out var cursorPos);
        
        Cursor.SetPos(cursorPos);
        _cursorFlashClock = 0;
        Cursor.Flags.Remove("Invisible");
        
        // Clip Text
        if (_netTextWidth != 0)
        {
            TextWriter.SetTexCoords(_textOffset / (float)_netTextWidth, (_textOffset+BoxWidth) / (float)_netTextWidth);
        }
    }

    private void AlignText(bool keepHighlight, out Vector2i cursorCoord)
    {
        // Ensure maximum text visible
        if (_netTextWidth - _textOffset < BoxWidth && _textOffset > 0)
        {
            _textOffset = _netTextWidth - BoxWidth;
        }
        
        // Get Cursor Pos
        var cursorXPos = CharWidths[..CursorLoc].Sum() - _textOffset;
        
        // Keep Cursor in box
        if (cursorXPos > BoxWidth)
        {
            _textOffset += cursorXPos - BoxWidth;
            cursorXPos = BoxWidth;
        }
        if (cursorXPos < 0)
        {
            _textOffset += cursorXPos;
            cursorXPos = 0;
        }

        if (keepHighlight)
        {
            var highlightStart = CharWidths[..Math.Min(CursorLoc, HighlightAnchor)].Sum() - _textOffset;
            var highlightEnd = CharWidths[..Math.Max(CursorLoc, HighlightAnchor)].Sum() - _textOffset;

            // Ensure highlight box stays in text box
            if (highlightStart < 0)
            {
                highlightStart = 0;
            }
            if (highlightEnd > BoxWidth)
            {
                highlightEnd = BoxWidth;
            }
            
            if (!Flags.Contains("HighlightText"))
            {
                Cursor.UpdateTexture(CursorTexHighlight);
                Flags.Add("HighlightText");
            }
            Cursor.Resize(highlightEnd - highlightStart, Cursor.Size.Y);
            cursorXPos = highlightStart;
        }
        else
        {
            // Remove highlight
            HighlightAnchor = CursorLoc;
            
            if (Flags.Contains("HighlightText"))
            {
                Cursor.UpdateTexture(CursorTexStandard);
                Flags.Remove("HighlightText");
            }
        }

        cursorCoord = new Vector2i(cursorXPos, Cursor.Pos.Y);
    }
}