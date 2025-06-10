namespace PokeEngine.Core.Tweening;

public static class Easing
{
    public static float Linear(float t) => t;
    public static float InQuad(float t) => t * t;
    public static float OutQuad(float t) => t * (2 - t);
    public static float InOutQuad(float t) =>
        t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
}