namespace PokeSharp.Engine.Graphics;

public class Animation
{
    public bool IsLooping { get; }
    public float Frequency { get; }
    public float InverseFrequency { get; }
    public int FramesCount { get; }
    public IReadOnlyList<Sprite> Sprites { get; }

    public Animation(List<Sprite> sprites, float frequency = 12.0f, bool isLooping = true)
    {
        Sprites = sprites;
        Frequency = frequency;
        InverseFrequency = 1f / frequency;
        IsLooping = isLooping;
        FramesCount = Sprites.Count;
    }

    public static Animation FromSpriteSheet(SpriteSheet sheet, int index)
        => FromSpriteSheet(sheet, index, index, 0.0f, false);

    public static Animation FromSpriteSheet(SpriteSheet sheet, int from, int to, float frequency = 10.0f, bool isLooping = true)
    {
        var sprites = new List<Sprite>();
        for (int i = from; i <= to; i++)
            sprites.Add(sheet.GetSprite(i));

        return new Animation(sprites, frequency, isLooping);
    }
}