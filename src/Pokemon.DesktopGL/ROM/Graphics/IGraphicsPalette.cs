using Microsoft.Xna.Framework;

namespace Pokemon.DesktopGL.ROM.Graphics;

public interface IGraphicsPalette
{
    int ColorCount { get; }

    Color[] Data { get; }
}