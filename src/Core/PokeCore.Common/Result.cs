using System.Diagnostics.CodeAnalysis;
using PokeCore.Diagnostics;

namespace PokeCore.Common;

public record Error(string Message);

public abstract record Result
{
    public bool IsSuccess => this is SuccessResult;
    public bool IsFailure => this is FailureResult;

    public sealed record SuccessResult : Result;
    public sealed record FailureResult(Error Error) : Result;

    public Error GetError()
    {
        ThrowHelper.Assert(IsFailure, "This result represents a failure and therefore cannot get success value.");
        return ((FailureResult)this).Error;
    }

    public bool TryGetError([NotNullWhen(true)] out Error? error)
    {
        error = default;

        if (this is not FailureResult failure)
            return false;

        error = failure.Error!;
        return true;
    }

    public static Result Success()
        => new SuccessResult();

    public static Task<Result> SuccessAsync()
        => Task.FromResult(Success());

    public static Result Failure(Error error)
        => new FailureResult(error);

    public static Task<Result> FailureAsync(Error error)
        => Task.FromResult(Failure(error));

    public static implicit operator Result(Error error)
        => new FailureResult(error);
}

public abstract record Result<T>
{
    public bool IsSuccess => this is SuccessResult;
    public bool IsFailure => this is FailureResult;

    public sealed record SuccessResult(T Value) : Result<T>;
    public sealed record FailureResult(Error Error) : Result<T>;

    public T GetValue()
    {
        ThrowHelper.Assert(IsSuccess, "This result represents a failure and therefore cannot get success value.");
        return ((SuccessResult)this).Value;
    }

    public Error GetError()
    {
        ThrowHelper.Assert(IsFailure, "This result represents a failure and therefore cannot get success value.");
        return ((FailureResult)this).Error;
    }

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = default;

        if (this is not SuccessResult success)
            return false;

        value = success.Value!;
        return true;
    }

    public bool TryGetError([NotNullWhen(true)] out Error? error)
    {
        error = default;

        if (this is not FailureResult failure)
            return false;

        error = failure.Error!;
        return true;
    }


    public static Result<T> Success(T value)
        => new SuccessResult(value);

    public static Task<Result<T>> SuccessAsync(T value)
        => Task.FromResult(Success(value));

    public static Result<T> Failure(Error error)
        => new FailureResult(error);

    public static Task<Result<T>> FailureAsync(Error error)
        => Task.FromResult(Failure(error));

    public static implicit operator Result<T>(T result)
        => new SuccessResult(result);

    public static implicit operator Result<T>(Error error)
        => new FailureResult(error);
}