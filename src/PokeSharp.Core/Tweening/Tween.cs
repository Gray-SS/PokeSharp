using System;
using System.Collections;
using Microsoft.Xna.Framework;
using PokeSharp.Core.Coroutines;
using PokeSharp.Core.Managers;

namespace PokeSharp.Core.Tweening;

public delegate float TweenEase(float t);
public delegate T TweenInterpolator<T>(T from, T to, float t);

public class Tween<T> : ICoroutine
{
    public T From { get; }
    public T To { get; }
    public float Duration { get; }
    public TweenEase Ease { get; set; }
    public TweenInterpolator<T> Interpolator { get; }

    public bool IsStarted { get; private set; }
    public bool IsRunning { get; private set; }

    private float _elapsed;
    private readonly Action<T> _setter;

    public Tween(T to, float duration, Action<T> setter, Func<T> getter, TweenEase? ease, TweenInterpolator<T> interpolator)
    {
        _setter = setter;

        To = to;
        From = getter.Invoke();
        Duration = duration;
        Ease = ease ?? Easing.Linear;
        Interpolator = interpolator;
    }

    public void Start()
    {
        if (!IsRunning) return;

        CoroutineManager.Start(StartRoutine());
    }

    public IEnumerator StartRoutine()
    {
        IsRunning = true;

        yield return this;

        IsRunning = false;
    }

    public bool IsFinished(float dt)
    {
        _elapsed += dt;

        float t = Math.Clamp(_elapsed / Duration, 0f, 1f);
        float eased = Ease(t);

        T value = Interpolator.Invoke(From, To, eased);
        _setter?.Invoke(value);

        return _elapsed >= Duration;
    }
}

public static class Tween
{
    public static IEnumerator To(Action<float> setter, Func<float> getter, float to, float duration, TweenEase? ease = null)
    {
        return new Tween<float>(to, duration, setter, getter, ease, float.Lerp).StartRoutine();
    }

    public static IEnumerator To(Action<Vector2> setter, Func<Vector2> getter, Vector2 to, float duration, TweenEase? ease = null)
    {
        return new Tween<Vector2>(to, duration, setter, getter, ease, Vector2.Lerp).StartRoutine();
    }

    public static IEnumerator To(Action<Color> setter, Func<Color> getter, Color to, float duration, TweenEase? ease = null)
    {
        return new Tween<Color>(to, duration, setter, getter, ease, Color.Lerp).StartRoutine();
    }
}