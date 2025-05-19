using Microsoft.Xna.Framework;
using PokeSharp.Engine.Renderers;

namespace PokeSharp.Engine.UI;

public class UIContainer : UINode
{
    public IReadOnlyCollection<UINode> Children { get; set; }

    public UIContainer()
    {
        Children = [];
    }

    public override void Update(GameTime gameTime)
    {
        foreach (UINode child in Children)
            child.Update(gameTime);
    }

    public override void Arrange(Rectangle remainingSpace)
    {
        base.Arrange(remainingSpace);

        Rectangle contentArea = ApplyPadding(Bounds);

        int crntY = contentArea.Top;
        foreach (UINode node in Children)
        {
            var margin = node.Margin;
            int height = node.ComputeHeight(contentArea.Height - margin.Vertical);

            var childBounds = new Rectangle(
                contentArea.X + margin.Left,
                crntY + margin.Top,
                contentArea.Width - margin.Horizontal,
                height
            );

            node.Arrange(childBounds);

            crntY += height + margin.Vertical;
        }
    }

    public override void Draw(UIRenderer renderer)
    {
        base.Draw(renderer);

        foreach (UINode node in Children)
        {
            node.Draw(renderer);
        }
    }

    protected override int ComputeAutoWidth(int parentSize)
    {
        int maxContentWidth = ComputeMaxContentWidth(parentSize);
        return maxContentWidth + Padding.Horizontal;
    }

    protected override int ComputeAutoHeight(int parentSize)
    {
        int totalHeight = 0;

        foreach (UINode node in Children)
        {
            int height = node.ComputeHeight(parentSize - node.Margin.Vertical);
            totalHeight += height + node.Margin.Vertical;
        }

        return totalHeight + Padding.Vertical;
    }

    protected override int ComputeMaxContentWidth(int parentSize)
    {
        int maxWidth = 0;

        foreach (UINode node in Children)
        {
            if (node.Width.IsRelative && Width.IsAuto)
                throw new InvalidOperationException("Computation error: Can't use relative children width with an auto width");

            int width = node.ComputeWidth(parentSize);
            int totalWidth = width + node.Margin.Horizontal;

            if (totalWidth > maxWidth)
                maxWidth = totalWidth;
        }

        return maxWidth;
    }

    protected override int ComputeMaxContentHeight(int parentSize)
    {
        int totalHeight = 0;

        foreach (UINode node in Children)
        {
            if (node.Height.IsRelative && Height.IsAuto)
                throw new InvalidOperationException("Computation error: Can't use relative children height with an auto height");

            int height = node.ComputeHeight(parentSize);
            totalHeight += height + node.Margin.Vertical;
        }

        return totalHeight;
    }
}