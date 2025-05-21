namespace PokeSharp.Rendering.Assets.Animations;

public enum AnimationCmdType
{
    Frame,
    Loop,
    Jump,
    End
}

public interface IAnimationCmd
{
    AnimationCmdType Type { get; }
}

public sealed class AnimationCmdEnd : IAnimationCmd
{
    public AnimationCmdType Type => AnimationCmdType.End;

    public static readonly AnimationCmdEnd Default = new();
}

public sealed class AnimationCmdJump : IAnimationCmd
{
    public AnimationCmdType Type => AnimationCmdType.Jump;

    public int TargetCmdIndex { get; }

    public AnimationCmdJump(int target)
    {
        TargetCmdIndex = target;
    }
}

public sealed class AnimationCmdLoop : IAnimationCmd
{
    public AnimationCmdType Type => AnimationCmdType.Loop;

    public int Count { get; set; }

    public AnimationCmdLoop(int count)
    {
        Count = count;
    }
}

public sealed class AnimationCmdFrame : IAnimationCmd
{
    public AnimationCmdType Type => AnimationCmdType.Frame;

    /// <summary>
    /// This is corresponding to the images index
    /// </summary>
    public int FrameIndex { get; }

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

    public AnimationCmdFrame(int index, int duration, bool hflip, bool vflip)
    {
        FrameIndex = index;
        Duration = duration;
        HFlip = hflip;
        VFlip = vflip;
    }
}