using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using TGUI.Windowing;

namespace TGUI
{
    class Program
    {
        public static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = GameWindowSettings.Default;
            NativeWindowSettings nativeWindowSettings = NativeWindowSettings.Default;
            gameWindowSettings.UpdateFrequency = 160;
            nativeWindowSettings.Title = "TGUI APP";
            nativeWindowSettings.Location = new Vector2i(0,0);
            var app = new GuiWindow(gameWindowSettings, nativeWindowSettings);
            app.Run();

        }
    }
}