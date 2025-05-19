using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Managers;

namespace PokeSharp.Engine.Miscellaneous;

public class Camera
{
    public static Camera Active => PokesharpEngine.Instance.ScreenManager.ActiveScreen.Camera;

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public float Zoom
    {
        get => _zoom;
        set => _zoom = Math.Max(value, 0.1f);
    }

    public Viewport Viewport => _resolutionManager.VirtualViewport;

    private float _zoom = 1.0f;
    private readonly ResolutionManager _resolutionManager;

    public Camera(ResolutionManager resolutionManager)
    {
        _resolutionManager = resolutionManager;
    }

    public virtual Matrix TransformMatrix
    {
        get
        {
            var width = _resolutionManager.VirtualResolution.Width;
            var height = _resolutionManager.VirtualResolution.Height;

            return Matrix.CreateTranslation(new Vector3(-Position, 0))
                * Matrix.CreateScale(Zoom)
                * Matrix.CreateRotationZ(Rotation)
                * Matrix.CreateTranslation(new Vector3(width * 0.5f, height * 0.5f, 0));
        }
    }

    public Matrix InverseTransformMatrix => Matrix.Invert(TransformMatrix);

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, InverseTransformMatrix);
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, TransformMatrix);
    }
}