using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TGameToolkit.Objects;

public class Camera : GameObject
{
    public Vector3 Front = -Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;
    public Vector3 Right = Vector3.UnitX;
    private float _pitch;
    private float _yaw = -MathF.PI / 2;
    private float _fov = MathF.PI / 2;

    public float Speed = 10;
    public float AspectRatio;

    public float Pitch
    {
        get => _pitch / MathF.PI * 180f ;
        set
        {
            var angle = Math.Clamp(value, -89f, 89f);
            _pitch = angle * (MathF.PI / 180f);
            UpdateVectors();
        }
    }
    public float Yaw
    {
        get => _yaw  / MathF.PI * 180;
        set
        {
            _yaw = value * (MathF.PI / 180f);
            UpdateVectors();
        }
    }
    public float Fov
    {
        get => _fov / MathF.PI * 180;
        set => _fov = Math.Clamp(value, 1f, 45f)  * (MathF.PI / 180f);
        
    }
    
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Pos, Pos + Front, Up);
    }
    
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    public void MouseMove(Vector2 delta)
    {
        Yaw += delta.X/2;
        Pitch -= delta.Y/2;
    }
    
    public void Move(KeyboardState input, float deltaTime)
    {
        Pos -= input.IsKeyDown(Keys.W)? Vector3.Zero : Front * Speed * deltaTime/2;
        Pos += input.IsKeyDown(Keys.S)? Vector3.Zero : Front * Speed * deltaTime/2;
        Pos -= input.IsKeyDown(Keys.D)? Vector3.Zero : Vector3.Normalize(Vector3.Cross(Front, Up)) * Speed * deltaTime/2;
        Pos += input.IsKeyDown(Keys.A)? Vector3.Zero : Vector3.Normalize(Vector3.Cross(Front, Up)) * Speed * deltaTime/2;
        Pos -= input.IsKeyDown(Keys.Space) ? Vector3.Zero : Vector3.UnitY * Speed * deltaTime / 2;
        Pos += input.IsKeyDown(Keys.LeftShift)? Vector3.Zero : Vector3.UnitY * Speed * deltaTime/2;
    }
    
    private void UpdateVectors()
    {
        Front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        Front.Y = MathF.Sin(_pitch);
        Front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        Front = Vector3.Normalize(Front);
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY)); 
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
    
    public Camera()
    {
        Pos = (0, 0, 10);
    }
}