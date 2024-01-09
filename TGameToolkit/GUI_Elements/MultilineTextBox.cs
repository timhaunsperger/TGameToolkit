// using OpenTK.Mathematics;
// using TGameToolkit.Windowing;
//
// namespace TGameToolkit.GUI_Elements;
//
// public class MultilineTextBox : TextBox
// {
//     private int _multilineHighlightBoxes;
//     private int _lineHeight;
//     private List<int>? _lineBreaks;
//     
//     public MultilineTextBox(AppWindow window, Vector2i pos, Vector2i size, AlignMode align = AlignMode.Default, 
//         Shader? shader = null, Tex? boxTex = null, Vector4i? textColor = null, 
//         string defaultText = "defaultText", int textSize = 14, int capacity = 1000) 
//         : base(window, pos, size, align, boxTex, textColor)
//     {
//         throw new NotImplementedException();
//         _lineHeight = (int)(textSize * 1.2);
//         TextWriter.Align = AlignMode.UpperLeft;
//         Cursor.Align = AlignMode.UpperLeft;
//     }
//
//     private void AlignPosToText(Vector2i position, out int charIndexBefore)
//     {
//         var widthCounter = 0;
//         position.X -= TextWriter.BoundingBox.Min.X;
//         position.Y -= TextWriter.BoundingBox.Min.Y;
//         var line = 0;
//         Console.WriteLine(position.Y);
//         for (int i = 0; i < _lineBreaks.Count; i++)
//         {
//             position.Y -= _lineHeight;
//             if (position.Y > 0 && line < _lineBreaks.Count - 2)
//             {
//                 line += 1;
//             }
//         }
//         Console.WriteLine(position.Y);
//         for (var i = _lineBreaks[line]; i < Text.Length; i++)
//         {
//             if (position.X <= widthCounter + CharWidths[i] / 2)
//             {
//                 charIndexBefore = i;
//                 break;
//             }
//             widthCounter += CharWidths[i];
//         }
//
//         charIndexBefore = 0;
//     }
//
//     private void UpdateTextBox(bool keepHighlight = false)
//     {
//         // TextGenerator.GetTextBlockData(
//         //     Text,
//         //     _textSize,
//         //     BoxWidth,
//         //     TextColor,
//         //     out CharWidths,
//         //     out byte[] texData,
//         //     out int height,
//         //     out _lineBreaks);
//         //     
//         // TextWriter.UpdateTexture(new Tex(texData, BoxWidth, height));
//         // TextWriter.Resize(BoxWidth, height);
//         // AlignTextMultiLine(keepHighlight, out cursorPos);
//     }
//     
//     private int GetLine(int location)
//     {
//         for (int i = 0; i < _lineBreaks.Count - 1; i++)
//         {
//             if (location >= _lineBreaks[i] && location <= _lineBreaks[i + 1])
//             {
//                 return i;
//             }
//         }
//
//         return 0;
//     }
//     
//     private void AlignTextMultiLine(bool keepHighlight, out Vector2i cursorCoord)
//     {
//         var cursorLine = GetLine(CursorLoc);
//         var lineStart = _lineBreaks[cursorLine];
//         var cursorXPos = 0;
//         var cursorYPos = cursorLine * _lineHeight;
//
//         for (int i = lineStart; i < CursorLoc; i++)
//         {
//             cursorXPos += CharWidths[i];
//         }
//
//         cursorCoord = new Vector2i(cursorXPos, cursorYPos);
//         
//         // Reset multi-line highlight
//         for (int i = 0; i < _multilineHighlightBoxes; i++)
//         {
//             TextWriter.Children.Remove("HighlightBox" + i);
//         }
//         _multilineHighlightBoxes = 0;
//         
//         if (keepHighlight)
//         {
//             Flags.Add("HighlightText");
//             var highlightStartLoc = Math.Min(CursorLoc, HighlightAnchor);
//             var highlightEndLoc = Math.Max(CursorLoc, HighlightAnchor);
//             var highlightStartLine = GetLine(highlightStartLoc);
//             var highlightEndLine = GetLine(highlightEndLoc);
//             var highlightStartPos = CharWidths[_lineBreaks[highlightStartLine] .. highlightStartLoc].Sum();
//             var highlightEndPos = CharWidths[_lineBreaks[highlightEndLine] .. highlightEndLoc].Sum();
//             cursorCoord.X = highlightStartPos;
//             
//             if (highlightStartLine != highlightEndLine)
//             {
//                 _multilineHighlightBoxes = highlightEndLine - highlightStartLine + 1;
//                 Cursor.UpdateTexture(Tex.Blank);
//                 
//                 for (int i = 0; i < _multilineHighlightBoxes; i++)
//                 {
//                     if (Children.ContainsKey("HighlightBox" + i)) return;
//                     
//                     var boxPos = i == 0 ? highlightStartPos : 0;
//                     int boxWidth;
//                     
//                     if (i == 0) boxWidth = BoxWidth - highlightStartPos;
//                     else if (i == _multilineHighlightBoxes - 1) boxWidth = highlightEndPos;
//                     else boxWidth = BoxWidth;
//
//                     TextWriter.AddChild(
//                         "HighlightBox" + i, 
//                         new Element(
//                             Window, (boxPos, _lineHeight * (highlightStartLine + i)), 
//                             Shader.UiShader, CursorTexHighlight, 
//                             AlignMode.UpperLeft, (boxWidth, _lineHeight)));
//                 }
//             }
//             else
//             {
//                 // Standard Highlight
//                 Cursor.UpdateTexture(CursorTexHighlight);
//                 Cursor.Resize(highlightEndPos - highlightStartPos, _lineHeight);
//             }
//             
//         }
//         else
//         {
//             Flags.Remove("HighlightText");
//             // Remove highlight
//             HighlightAnchor = CursorLoc;
//             Cursor.UpdateTexture(CursorTexStandard);
//         }
//     }
//     
// }