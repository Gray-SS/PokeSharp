using System.Drawing;

namespace PokeTools.ROM.Graphics;

public interface IRomPalette : IIndexedResource
{
    Color[] Data { get; }
}