using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using TTKGui.Windowing;

namespace TTKGui
{
    class Program
    {
        public static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = GameWindowSettings.Default;
            NativeWindowSettings nativeWindowSettings = NativeWindowSettings.Default;
            gameWindowSettings.UpdateFrequency = 160;
            nativeWindowSettings.Title = "TTKGui App";
            nativeWindowSettings.Location = new Vector2i(0,0);
            var app = new GuiWindow(gameWindowSettings, nativeWindowSettings);
            app.Run();
            
        }
    }
}