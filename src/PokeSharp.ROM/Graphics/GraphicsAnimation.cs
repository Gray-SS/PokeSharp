namespace PokeSharp.ROM.Graphics;

public readonly struct GraphicsAnimation
{
    /// <summary>
    /// This is corresponding to the images index
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The duration in ms
    /// </summary>
    public int Duration { get; }

    /// <summary>
    /// Tells if the sprite has to be flipped horizontally
    /// </summary>
    public bool HFlip { get; }

    /// <summary>
    /// Tells if the sprite has to be flipped vertically
    /// </summary>
    public bool VFlip { get; }
}