namespace PokeEngine.Rendering.Animations;

public class Animation
{
    public IReadOnlyList<Sprite> Frames { get; }
    public IReadOnlyList<IAnimationCmd> Commands { get; }

    public Animation(List<Sprite> frames, List<IAnimationCmd> commands)
    {
        Frames = frames;
        Commands = commands;
    }

    public static Animation FromSpriteSheet(SpriteSheet sheet, int index)
        => FromSpriteSheet(sheet, index, index, 4, true);

    public static Animation FromSpriteSheet(SpriteSheet sheet, int from, int to, int frameDuration, bool isLooping = true)
    {
        var frames = new List<Sprite>();
        var cmds = new List<IAnimationCmd>();
        for (int i = from; i <= to; i++)
        {
            frames.Add(sheet.GetSprite(i));
            cmds.Add(new AnimationCmdFrame(i - from, frameDuration, false, false));
        }

        if (isLooping)
            cmds.Add(new AnimationCmdJump(0));

        return new Animation(frames, cmds);
    }
}