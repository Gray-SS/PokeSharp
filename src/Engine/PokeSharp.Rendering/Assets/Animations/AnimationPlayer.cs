namespace PokeSharp.Rendering.Assets.Animations;

public sealed class AnimationPlayer
{
    public bool IsPlaying { get; private set; }
    public Animation? Animation { get; private set; }
    public bool FlipH => Command is AnimationCmdFrame animFrame && animFrame.HFlip;
    public bool FlipV => Command is AnimationCmdFrame animFrame && animFrame.VFlip;
    public Sprite? Frame => Animation?.Frames[_frameIndex];
    public IAnimationCmd? Command => Animation?.Commands[_cmdIndex];

    private float _timer;
    private int _cmdIndex;
    private int _frameIndex;

    public void Play(Animation animation)
    {
        IsPlaying = true;
        Animation = animation;

        _timer = 0.0f;
        _frameIndex = 0;
        UpdateCmdIndex(0);
    }

    public void Replay()
    {
        IsPlaying = true;

        _cmdIndex = 0;
        _frameIndex = 0;
        _timer = 0.0f;
    }

    public void Pause()
    {
        IsPlaying = false;
    }

    public void Stop()
    {
        if (!IsPlaying) return;

        _timer = 0.0f;
        _cmdIndex = 0;
        _frameIndex = 0;
        IsPlaying = false;
    }

    public void Update(float dt)
    {
        if (!IsPlaying || Command == null || Animation == null)
            return;

        switch (Command)
        {
            case AnimationCmdFrame frameCmd:
                UpdateFrameCmd(frameCmd, dt);
                break;
        }
    }

    private void UpdateCmdIndex(int newCmdIndex)
    {
        _cmdIndex = newCmdIndex;
        if (_cmdIndex >= Animation?.Commands.Count)
            return;

        switch (Command)
        {
            case AnimationCmdJump jumpCmd:
                UpdateCmdIndex(jumpCmd.TargetCmdIndex);
                break;
            case AnimationCmdFrame frameCmd:
                _frameIndex = frameCmd.FrameIndex;
                break;
            case AnimationCmdLoop loopCmd:
                if (loopCmd.Count <= 0)
                    return;

                loopCmd.Count--;
                UpdateCmdIndex(0);
                break;
            case AnimationCmdEnd:
                Stop();
                break;
        }
    }

    private void UpdateFrameCmd(AnimationCmdFrame frameCmd, float dt)
    {
        _timer += dt;
        if (_timer >= frameCmd.Duration * 0.02f)
        {
            _timer = 0.0f;
            UpdateCmdIndex(_cmdIndex + 1);
        }
    }
}