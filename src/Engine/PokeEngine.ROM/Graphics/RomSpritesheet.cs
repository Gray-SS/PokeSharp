namespace PokeEngine.ROM.Graphics;

public readonly struct RomSpriteSheet
{
    public int Rows { get; }
    public int Columns { get; }
    public IRomTexture Texture { get; }

    public RomSpriteSheet(IRomTexture image, int cols, int rows)
    {
        Texture = image;
        Rows = rows;
        Columns = cols;
    }
}