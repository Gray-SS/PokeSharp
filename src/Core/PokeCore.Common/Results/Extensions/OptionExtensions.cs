namespace PokeCore.Common.Results.Extensions;

public static class OptionExtensions
{
    public static T GetValueOrDefault<T>(this Option<T> option, T defaultValue)
    {
        return option switch
        {
            Some<T>(T value) => value,
            None<T> => defaultValue,
            _ => throw new NotSupportedException()
        };
    }

    public static T GetValueOrDefault<T>(this Option<T> option, Func<T> defaultFactoryValue)
    {
        return option switch
        {
            Some<T>(T value) => value,
            None<T> => defaultFactoryValue.Invoke(),
            _ => throw new NotSupportedException()
        };
    }

    public static Option<TOut> Cast<TIn, TOut>(this Option<TIn> result)
        where TIn : TOut
    {
        return result switch
        {
            Some<TIn>(TIn value) => new Some<TOut>(value),
            None<TIn> => new None<TOut>(),
            _ => throw new NotSupportedException()
        };
    }

    public static Option<TOut> Map<TIn, TOut>(this Option<TIn> result, Func<TIn, Option<TOut>> func)
    {
        return result switch
        {
            Some<TIn>(TIn value) => func.Invoke(value),
            None<TIn> => new None<TOut>(),
            _ => throw new NotSupportedException()
        };
    }

    public static Option<TOut> Map<TIn, TOut>(this Option<TIn> result, Func<TIn, TOut> func)
    {
        return result switch
        {
            Some<TIn>(TIn value) => new Some<TOut>(func.Invoke(value)),
            None<TIn> => new None<TOut>(),
            _ => throw new NotSupportedException()
        };
    }

    public static void Match(this IOption<Unit> result, Action onSome, Action onNone)
    {
        switch (result)
        {
            case Some<Unit>:
                onSome.Invoke();
                break;

            case None<Unit>:
                onNone.Invoke();
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public static void Match<TIn>(this Option<TIn> result, Action<TIn> onSome, Action onNone)
    {
        switch (result)
        {
            case Some<TIn>(TIn value):
                onSome.Invoke(value);
                break;

            case None<TIn>:
                onNone.Invoke();
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public static TOut Reduce<TIn, TOut>(this Option<TIn> result, Func<TIn, TOut> onSome, Func<TOut> onNone)
    {
        return result switch
        {
            Some<TIn>(TIn value) => onSome.Invoke(value),
            None<TIn> => onNone.Invoke(),
            _ => throw new NotSupportedException(),
        };
    }

    public static Option<U> Select<T, U>(this Option<T> result, Func<T, U> selector)
        => result switch
        {
            Some<T>(var value) => selector(value),
            None<T> => new None<U>(),
            _ => throw new InvalidOperationException()
        };

    public static Option<U> SelectMany<T, U>(
        this Option<T> result,
        Func<T, Option<U>> selector)
        => result switch
        {
            Some<T>(var value) => selector(value),
            None<T> => new None<U>(),
            _ => throw new InvalidOperationException()
        };

    public static Option<V> SelectMany<T, U, V>(
        this Option<T> result,
        Func<T, Option<U>> bind,
        Func<T, U, V> project)
        => result.SelectMany(
            t => bind(t).Select(u => project(t, u))
        );
}