namespace PokeSharp.Engine.UI;

public readonly struct UISize
{
    public float Value { get; }
    public Units Unit { get; }

    public bool IsAuto => Unit == Units.Auto;
    public bool IsFixed => Unit == Units.Pixels;
    public bool IsRelative => Unit is Units.Scalar or Units.ViewportScalar;

    public static readonly UISize Auto = new(Units.Auto, 0.0f);
    public static readonly UISize MaxContent = new(Units.MaxContent, 0.0f);

    public UISize(Units unit, float value)
    {
        Unit = unit;
        Value = value;
    }

    public static UISize Pixels(int value)
        => new UISize(Units.Pixels, value);

    public static UISize Scalar(float scalar)
        => new UISize(Units.Scalar, scalar);

    public static UISize ViewportScalar(float scalar)
        => new UISize(Units.ViewportScalar, scalar);

    public enum Units
    {
        Auto,
        Pixels,
        Scalar,
        ViewportScalar,
        MaxContent
    }
}