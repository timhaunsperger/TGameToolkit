using System.Reflection;
using OpenTK.Mathematics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TGameToolkit.Drawing;

namespace TGameToolkit.GUI_Elements.Text;

public static class TextGenerator
{
    private static FontCollection _fonts = new ();
    private static FontFamily _defaultFont;
    private static Dictionary<string, Font> _fontStash = new ();
    
    private static Dictionary<string, Tuple<int, int, byte[]>> _glyphStash = new ();

    static TextGenerator()
    {
        var assembly = Assembly.GetExecutingAssembly();
        _defaultFont = _fonts.Add(assembly.GetManifestResourceStream("TGameToolkit.GUI_Elements.Text.OpenSans-Medium.ttf"));
    }
    
    public static void AddFont(string fontPath)
    {
        _fonts.Add(fontPath);
    }
    
    /// <summary>
    /// Default fonts included are OpenSans-Medium.ttf and OpenSans-SemiBold.ttf
    /// </summary>
    /// <param name="fontName"></param>
    public static void SetDefaultFont(string fontName)
    {
        if (!_fonts.TryGet(fontName, out _defaultFont))
        {
            throw new FontFamilyNotFoundException(fontName);
        }
    }
    /// <summary>
    /// Get glyph for a string formatted as array bytes of pixel format A8.
    /// </summary>
    /// <param name="text">Text to get glyph data for</param>
    /// <param name="textOptions">Text options from font</param>
    /// <param name="size">Text size in pt</param>
    /// <returns>Tuple of (width, height, data).</returns>
    private static Tuple<int, int, byte[]> GetGlyph(string text, TextOptions textOptions, int size)
    {
        var glyphId = text + size + textOptions.Font.Name;
        if (_glyphStash.TryGetValue(glyphId, out var glyph))
        {
            return glyph;
        }
        
        var width = TextMeasurer.MeasureAdvance(text, textOptions).Width;
        var height = size * 1.2;
        if (width == 0)
        {
            glyph =  new Tuple<int, int, byte[]>((int)width, (int)height, Array.Empty<byte>());
            _glyphStash.Add(glyphId, glyph);
            return glyph;
        }
        
        IPathCollection charShape = TextBuilder.GenerateGlyphs(text, textOptions);
        var image = new Image<A8>((int)width, (int)height);
        image.Mutate(x => x.Fill(new Rgba32(0f, 0f, 0f, 0f)));
        image.Mutate(x => x.Fill(new Rgba32(1f, 1f, 1f), charShape));
        
        var imgData = new byte[(int)width * (int)height];
        
        image.CopyPixelDataTo(imgData);
        glyph = new Tuple<int, int, byte[]>((int)width, (int)height, imgData);
        _glyphStash.Add(glyphId, glyph);
        return glyph;
    }

    public static void GetStringData(
        string text, int size, Vector4i color, out int[] charWidths, out byte[] imgData, out int netWidth, out int height)
    {
        if (!_fontStash.TryGetValue(_defaultFont.Name + size, out var font))
        {
            font = _defaultFont.CreateFont(size);
            _fontStash[_defaultFont.Name + size] = font;
        }

        var options = new TextOptions(font);
        var colorBytes = new[] { (byte)color[0], (byte)color[1], (byte)color[2], (byte)color[3] };

        netWidth = 0;
        height = (int)(size * 1.2);
        charWidths = new int[text.Length];
        var chars = new Tuple<int, int, byte[]>[text.Length];
        
        for (int i = 0; i < text.Length; i++)
        {
            var character = text[i].ToString();

            var charImgData = GetGlyph(character, options, size);
            netWidth += charImgData.Item1;
            charWidths[i] = charImgData.Item1;
            chars[i] = charImgData;
        }

        if (netWidth == 0)
        {
            imgData = new byte[] { 100, 0, 0, 0 };
            return;
        }
        
        var netByteWidth = 4 * netWidth;
        imgData = new byte[netByteWidth * height];

        for (int row = 1; row <= height; row++)
        {
            var offset = 0;
            var lineIndex = netByteWidth * (height - row);
            foreach (var (charWidth, charHeight, glyphData) in chars)
            {
                for (int j = 0; j < charWidth; j++)
                {
                    if (row < charHeight)
                    {
                        imgData[lineIndex + offset + j * 4] = colorBytes[0];
                        imgData[lineIndex + offset + j * 4 + 1] = colorBytes[1];
                        imgData[lineIndex + offset + j * 4 + 2] = colorBytes[2];
                        imgData[lineIndex + offset + j * 4 + 3] = (byte)((colorBytes[3] * glyphData[charWidth * row + j]) >> 8);
                    }
                }

                offset += charWidth * 4;
            }
        }
    }
    
    public static Texture GetStringTex(string text, int size, Vector4i color)
    {
        GetStringData(text, size, color, out _, out byte[] texData, out int netWidth, out int height);
        
        // Prevent black box for empty texture
        if (netWidth == 0)
        {
            netWidth = 1;
        }
        
        return new Texture(texData, netWidth, height);
    }

    public static void GetTextBlockData(string text, int textSize, int maxWidth, Vector4i color, out int[] charWidths, 
        out byte[] imgData, out int netHeight, out List<int> lineBreaks)
    {
        if (!_fontStash.TryGetValue(_defaultFont.Name + textSize, out var font))
        {
            font = _defaultFont.CreateFont(textSize);
            _fontStash[_defaultFont.Name + textSize] = font;
        }

        var lineHeight = (int)(textSize * 1.2);
        var options = new TextOptions(font);
        var lineWidth = 0;
        var chars = new Tuple<int, int, byte[]>[text.Length];
        charWidths = new int[text.Length];
        lineBreaks = new List<int>{0};
        netHeight = lineHeight; // There is always 1 line at least
        
        // Get layout info and glyph data
        for (int i = 0; i < text.Length; i++)
        {
            var character = text[i].ToString();
            var charImgData = GetGlyph(character, options, textSize);
            
            lineWidth += charImgData.Item1;
            charWidths[i] = charImgData.Item1;
            chars[i] = charImgData;
            if (lineWidth > maxWidth || text[i] == '\n')
            {
                lineBreaks.Add(i);
                lineWidth = charImgData.Item1;
                netHeight += lineHeight;
            }
        }
        lineBreaks.Add(text.Length);
        
        var r = (byte)color[0];
        var g = (byte)color[1];
        var b = (byte)color[2];
        var a = (byte)color[3];
        imgData = new byte[netHeight * maxWidth * 4];
        
        for (int row = 0; row < netHeight; row++)
        {
            var offset = 0;
            var lineNum = row / lineHeight;
            var rowIndex = maxWidth * 4 * (netHeight - (row+1));
            var lineRow = row - lineNum * lineHeight;
            foreach (var (charWidth, charHeight, glyphData) in chars[lineBreaks[lineNum] .. lineBreaks[lineNum + 1]])
            {
                
                for (int j = 0; j < charWidth; j++)
                {
                    if (lineRow < charHeight)
                    {
                        imgData[rowIndex + offset + j * 4] = r;
                        imgData[rowIndex + offset + j * 4 + 1] = g;
                        imgData[rowIndex + offset + j * 4 + 2] = b;
                        imgData[rowIndex + offset + j * 4 + 3] = (byte)((a * glyphData[charWidth * lineRow + j]) >> 8);
                    }
                }

                offset += charWidth * 4;
            }
        }
        
    }
}