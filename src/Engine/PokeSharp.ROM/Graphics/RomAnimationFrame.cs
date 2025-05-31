namespace PokeSharp.ROM.Graphics;

public enum RomAnimationCmdType
{
    Frame,
    Loop,
    Jump,
    End
}

public sealed class RomAnimation
{
    public IRomAnimationCmd[] Commands { get; }

    public RomAnimation(IRomAnimationCmd[] commands)
    {
        Commands = commands;
    }
}

public interface IRomAnimationCmd
{
    RomAnimationCmdType Type { get; }
}

public sealed class RomAnimationCmdEnd : IRomAnimationCmd
{
    public RomAnimationCmdType Type => RomAnimationCmdType.End;

    public static readonly RomAnimationCmdEnd Default = new();
}

public sealed class RomAnimationCmdJump : IRomAnimationCmd
{
    public RomAnimationCmdType Type => RomAnimationCmdType.Jump;

    public int Target { get; }

    public RomAnimationCmdJump(int target)
    {
        Target = target;
    }
}

public sealed class RomAnimationCmdLoop : IRomAnimationCmd
{
    public RomAnimationCmdType Type => RomAnimationCmdType.Loop;

    public int Count { get; }

    public RomAnimationCmdLoop(int count)
    {
        Count = count;
    }
}

public sealed class RomAnimationCmdFrame : IRomAnimationCmd
{
    public RomAnimationCmdType Type => RomAnimationCmdType.Frame;

    /// <summary>
    /// This is corresponding to the images index
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The duration in frames
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

    public RomAnimationCmdFrame(int index, int duration, bool hflip, bool vflip)
    {
        Index = index;
        Duration = duration;
        HFlip = hflip;
        VFlip = vflip;
    }
}