namespace PokeCore.Common.Results;

public sealed record Success<T>(T Value) : Result<T>;
public sealed record Failure<T>(string Message) : Result<T>;

public interface IResult<T>
{
    bool IsSuccess { get; }
    bool IsFailure { get; }

    T GetValue();
    string GetError();
}

public static class Result
{
    public static Success<T> Success<T>(T value)
        => new Success<T>(value);

    public static Success<Unit> Success()
        => new Success<Unit>(Unit.Value);

    public static Failure<Unit> Failure<T>(Failure<T> failure)
        => new Failure<Unit>(failure.Message);

    public static Failure<Unit> Failure(string message)
        => new Failure<Unit>(message);

    public static Result<T> FromNullable<T>(T? value, string errorMessage)
        where T : class
    {
        if (value != null)
            return Success(value);

        return Failure(errorMessage);
    }

    public static Result<T> FromNullable<T>(T? value, string errorMessage)
        where T : struct
    {
        if (value != null)
            return Success(value.Value);

        return Failure(errorMessage);
    }

    public static Result<Unit> Try(Action action)
    {
        try
        {
            action();
            return new Success<Unit>(Unit.Value);
        }
        catch (Exception ex)
        {
            return new Failure<Unit>(ex.Message);
        }
    }

    public static Result<T> Try<T>(Func<T> func)
    {
        try
        {
            return new Success<T>(func.Invoke());
        }
        catch (Exception ex)
        {
            return new Failure<T>(ex.Message);
        }
    }
}

public abstract record Result<T> : IResult<T>
{
    public bool IsSuccess => this is Success<T>;
    public bool IsFailure => this is Failure<T>;

    public Failure<T> Error => this is Failure<T> error ? error :
                                    throw new InvalidOperationException("Couldn't get result error. Operation suceeded.");

    public T GetValue()
        => this is Success<T> success ?
           success.Value :
           throw new InvalidOperationException("Couldn't get result value. Operation failed.");

    public string GetError()
        => this is Failure<T> failure ?
           failure.Message :
           throw new InvalidOperationException("Couldn't get result error. Operation suceeded.");

    public Result<Unit> ToUnit()
    {
        return this switch
        {
            Success<T> => new Success<Unit>(Unit.Value),
            Failure<T> failure => new Failure<Unit>(failure.Message),
            _ => throw new NotSupportedException()
        };
    }

    public Result<TOut> TryCast<TOut>()
    {
        return this switch
        {
            Success<T> some when some.Value is TOut val => new Success<TOut>(val),
            Failure<T> err => new Failure<TOut>(err.Message),
            _ => new Failure<TOut>($"Cannot cast {typeof(T)} to {typeof(TOut)}")
        };
    }

    public static implicit operator Result<T>(T value)
        => new Success<T>(value);

    public static implicit operator Result<T>(Failure<Unit> error)
        => new Failure<T>(error.Message);
}
