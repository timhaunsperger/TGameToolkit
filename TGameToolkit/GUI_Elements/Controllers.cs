using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TGameToolkit.Graphics;
using TGameToolkit.Objects;
using TGameToolkit.Windowing;

namespace TGameToolkit.GUI_Elements;

public class ObjectController : Panel
{
    
    public ObjectController(AppWindow window, Vector2i pos, GameObject obj)
        : base(window, pos, 400, obj.GetType().ToString())
    {
        foreach (var field in obj.GetType().GetFields())
        {
            if (field.FieldType == typeof(int))
            {
                var val = (int?)field.GetValue(obj) ?? 0;
                var slider = new Slider(window, Vector2i.Zero, (100, SlotHeight), 0, val * 4, (int)field.GetValue(obj));
                slider.OnUpdate = () => {
                    field.SetValue(obj, (int)slider.Value);
                    obj.OnModify();
                };
                AddElement(slider, field.Name);
            }
            if (field.FieldType == typeof(float))
            {
                var val = (float?)field.GetValue(obj) ?? 0;
                var slider = new Slider(window, Vector2i.Zero, (100, SlotHeight), val < 0 ? val * 4 : 0, val >= 0 ? val * 4 : 0 * 4, (float)field.GetValue(obj), intSteps:false);
                slider.OnUpdate = () => {
                    field.SetValue(obj, slider.Value);
                    obj.OnModify();
                };
                AddElement(slider, field.Name);
            }
            
            if (field.FieldType == typeof(bool))
            {
                var val = (bool?)field.GetValue(obj) ?? false;
                var box = new Checkbox(window, Vector2i.Zero, SlotHeight, val);
                box.OnPress = () => {
                    field.SetValue(obj, box.IsChecked);
                    obj.OnModify();
                };
                AddElement(box, field.Name);
            }
        }
    }
}

public class ShaderController : Panel
{
    public ShaderController(AppWindow window, Vector2i pos, Shader shader)
        : base(window, pos, 400, shader.GetType().ToString())
    {
        GL.GetProgram(shader.Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        for (int i = 0; i < numberOfUniforms; i++)
        {
            shader.Use();
            var key = GL.GetActiveUniform(shader.Handle, i, out _, out var type) ?? "";

            switch (type)
            {
                case ActiveUniformType.Float:
                {
                    var slider = new Slider(window, Vector2i.Zero, (100, SlotHeight), 0, 20, 0, intSteps:false);
                    slider.OnUpdate = () => {
                        shader.Use();
                        shader.SetFloat(key, slider.Value);
                    };
                    AddElement(slider, key);
                    break;
                }
                case ActiveUniformType.Int:
                {
                    var slider = new Slider(window, Vector2i.Zero, (100, SlotHeight), 0, 50, 0, intSteps:true);
                    slider.OnUpdate = () => {
                        shader.Use();
                        shader.SetInt(key, (int)slider.Value);
                    };
                    AddElement(slider, key);
                    break;
                }
            }
        }
    }
}