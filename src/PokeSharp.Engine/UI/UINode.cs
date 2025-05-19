using Microsoft.Xna.Framework;
using PokeSharp.Engine.Graphics;
using PokeSharp.Engine.Renderers;

namespace PokeSharp.Engine.UI;

public abstract class UINode
{
    public Sprite Sprite { get; set; }

    public UIThickness Margin { get; set; }
    public UIThickness Padding { get; set; }
    public UISize Width { get; set; } = UISize.Auto;
    public UISize Height { get; set; } = UISize.Auto;
    public Color BackgroundColor { get; set; } = Color.Transparent;
    public UIAnchorPoint Anchor { get; set; } = UIAnchorPoint.TopLeft;

    public Rectangle Bounds { get; protected set; }

    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;

    protected UINode()
    {
        Sprite = UIManager.Instance.FrameBackground;
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Arrange(Rectangle remainingSpace)
    {
        if (!IsVisible) return;

        Point computedSize = ComputeSize(remainingSpace.Size);
        Point location = ComputeAnchorLocation(in computedSize, in remainingSpace);

        Bounds = new Rectangle(location, computedSize);
    }

    public virtual void Draw(UIRenderer renderer)
    {
        if (!IsVisible) return;

        if (BackgroundColor != Color.Transparent && Sprite != null)
        {
            renderer.Draw(Sprite, Bounds, BackgroundColor);
        }
    }

    protected Point ComputeAnchorLocation(in Point computedSize, in Rectangle remainingSpace)
    {
        Point point = remainingSpace.Location;

        switch (Anchor)
        {
            case UIAnchorPoint.TopLeft:
                // Already at the correct location
                break;
            case UIAnchorPoint.TopCenter:
                point.X += (remainingSpace.Width - computedSize.X) / 2;
                break;
            case UIAnchorPoint.TopRight:
                point.X = remainingSpace.Right - computedSize.X;
                break;

            case UIAnchorPoint.CenterLeft:
                point.Y += (remainingSpace.Height - computedSize.Y) / 2;
                break;
            case UIAnchorPoint.Center:
                point.X += (remainingSpace.Width - computedSize.X) / 2;
                point.Y += (remainingSpace.Height - computedSize.Y) / 2;
                break;
            case UIAnchorPoint.CenterRight:
                point.X = remainingSpace.Right - computedSize.X;
                point.Y += (remainingSpace.Height - computedSize.Y) / 2;
                break;

            case UIAnchorPoint.BottomLeft:
                point.Y = remainingSpace.Bottom - computedSize.Y;
                break;
            case UIAnchorPoint.BottomCenter:
                point.X += (remainingSpace.Width - computedSize.X) / 2;
                point.Y = remainingSpace.Bottom - computedSize.Y;
                break;
            case UIAnchorPoint.BottomRight:
                point.X = remainingSpace.Right - computedSize.X;
                point.Y = remainingSpace.Bottom - computedSize.Y;
                break;
        }

        return point;
    }

    protected Point ComputeSize(Point parentSize)
    {
        int computedWidth = ComputeWidth(parentSize.X);
        int computedHeight = ComputeHeight(parentSize.Y);
        return new Point(computedWidth, computedHeight);
    }

    public int ComputeWidth(int parentSize)
    {
        return Width.Unit switch
        {
            UISize.Units.Pixels => (int)Width.Value,
            UISize.Units.Scalar => (int)(parentSize * Width.Value),
            UISize.Units.Auto => ComputeAutoWidth(parentSize),
            UISize.Units.MaxContent => ComputeMaxContentWidth(parentSize),
            _ => throw new NotImplementedException($"Failed to compute size: Units of type '{Width.Unit}' are currently not suppoorted."),
        };
    }

    public int ComputeHeight(int parentSize)
    {
        return Height.Unit switch
        {
            UISize.Units.Pixels => (int)Height.Value,
            UISize.Units.Scalar => (int)(parentSize * Height.Value),
            UISize.Units.Auto => ComputeAutoHeight(parentSize),
            UISize.Units.MaxContent => ComputeMaxContentHeight(parentSize),
            _ => throw new NotImplementedException($"Failed to compute size: Units of type '{Height.Unit}' are currently not supported."),
        };
    }

    protected virtual int ComputeAutoWidth(int parentSize)
    {
        return 0;
    }

    protected virtual int ComputeAutoHeight(int parentSize)
    {
        return 0;
    }

    protected virtual int ComputeMaxContentWidth(int parentSize)
    {
        return parentSize;
    }

    protected virtual int ComputeMaxContentHeight(int parentSize)
    {
        return parentSize;
    }

    protected Rectangle ApplyPadding(Rectangle bounds)
    {
        return new Rectangle(
            bounds.X + Padding.Left,
            bounds.Y + Padding.Top,
            bounds.Width - Padding.Horizontal,
            bounds.Height - Padding.Vertical
        );
    }
}