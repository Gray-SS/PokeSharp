using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Core.Managers;

namespace PokeSharp.Core;

public class Camera
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public float Zoom
    {
        get => _zoom;
        set => _zoom = Math.Max(value, 0.1f);
    }

    public Viewport Viewport => new Viewport(0, 0, _windowManager.WindowWidth, _windowManager.WindowHeight);

    private float _zoom = 1.0f;
    private readonly WindowManager _windowManager;

    public Camera(WindowManager windowManager)
    {
        _windowManager = windowManager;
    }

    public virtual Matrix TransformMatrix
    {
        get
        {
            var width = _windowManager.WindowWidth;
            var height = _windowManager.WindowHeight;

            return Matrix.CreateTranslation(new Vector3(-Position, 0))
                * Matrix.CreateScale(Zoom)
                * Matrix.CreateRotationZ(Rotation)
                * Matrix.CreateTranslation(new Vector3(width * 0.5f, height * 0.5f, 0));
        }
    }


    public Matrix InverseTransformMatrix => Matrix.Invert(TransformMatrix);

    public Rectangle GetWorldBounds()
    {
        var halfScreenSize = _windowManager.WindowSize * 0.5f;
        var topLeft = ScreenToWorld(-halfScreenSize);
        var bottomRight = ScreenToWorld(halfScreenSize);
        int width = (int)(bottomRight.X - topLeft.X);
        int height = (int)(bottomRight.Y - topLeft.Y);

        return new Rectangle((int)topLeft.X, (int)topLeft.Y, width, height);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, InverseTransformMatrix);
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, TransformMatrix);
    }
}