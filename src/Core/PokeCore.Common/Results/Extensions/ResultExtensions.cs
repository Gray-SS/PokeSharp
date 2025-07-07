namespace PokeCore.Common.Results.Extensions;

public static class ResultExtensions
{
    public static T GetValueOrDefault<T>(this Result<T> result, T defaultValue)
    {
        return result switch
        {
            Success<T>(T value) => value,
            Failure<T> => defaultValue,
            _ => throw new NotSupportedException()
        };
    }

    public static T GetValueOrDefault<T>(this Option<T> option, Func<T> defaultFactoryValue)
    {
        return option switch
        {
            Success<T>(T value) => value,
            Failure<T> => defaultFactoryValue.Invoke(),
            _ => throw new NotSupportedException()
        };
    }

    public static Result<TOut> Cast<TIn, TOut>(this Result<TIn> result)
        where TIn : TOut
    {
        return result switch
        {
            Success<TIn>(TIn value) => new Success<TOut>(value),
            Failure<TIn>(string message) => new Failure<TOut>(message),
            _ => throw new NotSupportedException()
        };
    }

    public static Result<TOut> AndThen<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func)
    {
        return result switch
        {
            Success<TIn>(TIn value) => Result.Success(func.Invoke(value)),
            Failure<TIn>(string message) => Result.Failure(message),
            _ => throw new NotSupportedException()
        };
    }

    public static Result<TOut> AndThen<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
    {
        return result switch
        {
            Success<TIn>(TIn value) => func.Invoke(value),
            Failure<TIn>(string message) => Result.Failure(message),
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<Result<TOut>> AndThen<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<TOut>> func)
    {
        return result switch
        {
            Success<TIn>(TIn value) => Result.Success(await func.Invoke(value)),
            Failure<TIn>(string message) => Result.Failure(message),
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<Result<TOut>> AndThen<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> func)
    {
        return result switch
        {
            Success<TIn>(TIn value) => await func.Invoke(value),
            Failure<TIn>(string message) => Result.Failure(message),
            _ => throw new NotSupportedException()
        };
    }

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
    {
        return result switch
        {
            Success<TIn>(TIn value) => func.Invoke(value),
            Failure<TIn>(string message) => new Failure<TOut>(message),
            _ => throw new NotSupportedException()
        };
    }

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func)
    {
        return result switch
        {
            Success<TIn>(TIn value) => new Success<TOut>(func.Invoke(value)),
            Failure<TIn>(string message) => new Failure<TOut>(message),
            _ => throw new NotSupportedException()
        };
    }

    public static void Match(this IResult<Unit> result, Action onSuccess, Action<string> onFailure)
    {
        switch (result)
        {
            case Success<Unit>:
                onSuccess.Invoke();
                break;

            case Failure<Unit>(string message):
                onFailure.Invoke(message);
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public static void Match<TIn>(this Result<TIn> result, Action<TIn> onSuccess, Action<string> onFailure)
    {
        switch (result)
        {
            case Success<TIn>(TIn value):
                onSuccess.Invoke(value);
                break;

            case Failure<TIn>(string message):
                onFailure.Invoke(message);
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public static TOut Reduce<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<string, TOut> onFailure)
    {
        return result switch
        {
            Success<TIn>(TIn value) => onSuccess.Invoke(value),
            Failure<TIn>(string message) => onFailure.Invoke(message),
            _ => throw new NotSupportedException(),
        };
    }

    public static Result<U> Select<T, U>(this Result<T> result, Func<T, U> selector)
        => result switch
        {
            Success<T>(var value) => selector(value),
            Failure<T>(var msg) => new Failure<U>(msg),
            _ => throw new InvalidOperationException()
        };

    public static Result<U> SelectMany<T, U>(
        this Result<T> result,
        Func<T, Result<U>> selector)
        => result switch
        {
            Success<T>(var value) => selector(value),
            Failure<T>(var msg) => new Failure<U>(msg),
            _ => throw new InvalidOperationException()
        };

    public static Result<V> SelectMany<T, U, V>(
        this Result<T> result,
        Func<T, Result<U>> bind,
        Func<T, U, V> project)
        => result.SelectMany(
            t => bind(t).Select(u => project(t, u))
        );
}