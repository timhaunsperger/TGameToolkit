using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TTKGui.Windowing;

namespace TTKGui.Text;
/// <summary>
/// This class allows reading and writing the windows system keyboard.
/// </summary>
public static unsafe class Clipboard
{
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool OpenClipboard(IntPtr hWin);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetClipboardData(uint uFormat);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool CloseClipboard();

    public static string GetClipboard(GuiWindow window)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "";
        IntPtr hWnd = GLFW.GetWin32Window(window.WindowPtr);
        OpenClipboard(hWnd);
        IntPtr dataPtr = GetClipboardData(13);
        CloseClipboard();
        
        var data = new byte[8];
        var length = 0;

        while (true)
        {
            if (length == data.Length)
            {
                Array.Resize(ref data, length * 2);
            }
            data[length] = Marshal.ReadByte(dataPtr, length);

            if (data[length] == 0 && data[length - 1] == 0)
            {
                break;
            }
            length++;
        }
        Array.Resize(ref data, length);
        return Encoding.Unicode.GetString(data);
    }
    
    public static void SetClipboard(GuiWindow window, string text)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        IntPtr hWnd = GLFW.GetWin32Window(window.WindowPtr);
        OpenClipboard(hWnd);

        byte[] textData = Encoding.Unicode.GetBytes(text + "\0");
        
        // Do not free this pointer as memory ownership is taken by the system when it is passed to SetClipboardData.
        // Attempting to free this pointer will cause a crash.
        IntPtr textPtr = Marshal.AllocHGlobal(textData.Length);
        Marshal.Copy(textData, 0, textPtr, textData.Length);
        
        SetClipboardData(13, textPtr);
        CloseClipboard();
    }
}