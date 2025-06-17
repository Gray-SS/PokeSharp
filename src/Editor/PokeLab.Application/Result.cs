using PokeCore.Diagnostics;

namespace PokeLab.Application;

public readonly struct Result
{
    public bool IsSuccess => FailureReason == null;
    public bool IsError => FailureReason != null;

    public string? FailureReason { get; }
    public static readonly Result Ok = new(null);

    private Result(string? errorMessage)
    {
        FailureReason = errorMessage;
    }

    public static Result Success()
    {
        return Ok;
    }

    public static Result Failed(string reason)
    {
        return new Result(reason);
    }
}

public readonly struct Result<T>
{
    public T Value { get; }
    public string? ErrorMessage { get; }

    public bool IsSuccess => ErrorMessage == null;
    public bool IsFailed => ErrorMessage != null;

    private Result(T value, string? errorMessage)
    {
        Value = value;
        ErrorMessage = errorMessage;
    }

    public static implicit operator Result<T>(T value)
        => Success(value);

    public static implicit operator T(Result<T> result)
    {
        ThrowHelper.Assert(result.IsSuccess, "Result must be in success state to access it's value implicitly.");
        return result.Value;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, null);
    }

    public static Result<T> Failed(string errorMessage)
    {
        return new Result<T>(default!, errorMessage);
    }
}