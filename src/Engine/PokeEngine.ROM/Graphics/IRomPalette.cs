using System.Drawing;

namespace PokeEngine.ROM.Graphics;

public interface IRomPalette : IIndexedResource
{
    Color[] Data { get; }
}