using System.Diagnostics.CodeAnalysis;

namespace PokeCore.Common.Results;

public sealed record Some<T>(T Value) : Option<T>;
public sealed record None<T> : Option<T>;

public interface IOption<T>
{
    bool HasValue { get; }

    T GetValue();
    bool TryGetValue(out T? value);
}

public static class Option
{
    public static readonly None<Unit> Empty = new();

    public static Some<T> With<T>(T value)
        => new(value);
}

public abstract record Option<T> : IOption<T>
{
    public bool HasValue => this is Some<T>;

    public T GetValue()
    {
        if (this is not Some<T>(T value))
            throw new InvalidOperationException("Cannot get option value.");

        return value;
    }

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = default;
        if (this is not Some<T>(T val))
            return false;

        value = val;
        return true;
    }

    public static implicit operator Option<T>(T value)
        => new Some<T>(value);

    public static implicit operator Option<T>(None<Unit> _)
        => new None<T>();
}