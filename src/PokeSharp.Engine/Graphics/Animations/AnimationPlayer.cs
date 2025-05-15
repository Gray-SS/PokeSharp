namespace PokeSharp.Engine.Graphics;

public sealed class AnimationPlayer
{
    public bool IsPlaying { get; private set; }
    public Animation? Animation { get; private set; }
    public Sprite? Sprite => Animation?.Sprites[_crntIndex];

    private float _timer;
    private int _crntIndex;

    public void Play(Animation animation)
    {
        IsPlaying = true;
        Animation = animation;

        _timer = 0.0f;
        _crntIndex = 0;
    }

    public void Replay()
    {
        IsPlaying = true;

        _crntIndex = 0;
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
        _crntIndex = 0;
        IsPlaying = false;
    }

    public void Update(float dt)
    {
        if (!IsPlaying || Animation == null)
            return;

        _timer += dt;
        if (_timer >= Animation.InverseFrequency)
        {
            _timer -= Animation.InverseFrequency;
            _crntIndex++;

            if (_crntIndex >= Animation.FramesCount)
            {
                if (!Animation.IsLooping)
                    Stop();

                _crntIndex %= Animation.FramesCount;
            }
        }
    }
}