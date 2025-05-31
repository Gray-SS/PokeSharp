using System.Drawing;

namespace PokeSharp.ROM.Graphics;

public interface IRomPalette : IIndexedResource
{
    Color[] Data { get; }
}