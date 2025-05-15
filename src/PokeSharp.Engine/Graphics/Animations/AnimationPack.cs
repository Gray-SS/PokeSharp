namespace PokeSharp.Engine.Graphics;

public sealed class AnimationPack
{
    public Animation this[string key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    private readonly Dictionary<string, Animation> _animations;

    public AnimationPack()
    {
        _animations = new Dictionary<string, Animation>();
    }

    public AnimationPack(Dictionary<string, Animation> animations)
    {
        _animations = animations;
    }

    public void Set(string key, Animation animation)
    {
        _animations[key] = animation;
    }

    public Animation Get(string key)
    {
        return _animations[key];
    }
}