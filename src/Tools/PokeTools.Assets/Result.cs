using System.Diagnostics.CodeAnalysis;
using PokeCore.Diagnostics;

namespace PokeTools.Assets;

public sealed record Unit;

public static class Result
{
    public static Result<TSuccess, Unit> Success<TSuccess>(TSuccess value)
    {
        return Result<TSuccess, Unit>.Succeeded(value);
    }

    public static Result<Unit, TError> Failed<TError>(TError error)
    {
        return Result<Unit, TError>.Failed(error);
    }
}

public abstract record Result<TSuccess, TError>
{
    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public sealed record Success(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Failure(TError Value) : Result<TSuccess, TError>;

    public Result<TResult, TError> Bind<TResult>(Func<TSuccess, TResult> next)
    {
        return next switch
        {
            Success success => new Result<TResult, TError>.Success(next.Invoke(success.Value)),
            Failure failure => new Result<TResult, TError>.Failure(failure.Value),
            _ => throw new NotImplementedException()
        };
    }

    public TSuccess GetValue()
    {
        ThrowHelper.Assert(IsSuccess, "This result represents a failure and therefore cannot get success value.");
        return ((Success)this).Value;
    }

    public TError GetError()
    {
        ThrowHelper.Assert(IsFailure, "This result represents a failure and therefore cannot get success value.");
        return ((Failure)this).Value;
    }

    public bool TryGetValue([NotNullWhen(true)] out TSuccess? value)
    {
        value = default;

        if (this is not Success success)
            return false;

        value = success.Value!;
        return true;
    }

    public bool TryGetError([NotNullWhen(true)] out TError? error)
    {
        error = default;

        if (this is not Failure failure)
            return false;

        error = failure.Value!;
        return true;
    }

    public static Success Succeeded(TSuccess Value)
        => new(Value);

    public static Failure Failed(TError Value)
        => new(Value);

    public static implicit operator Result<TSuccess, TError>(TSuccess result)
        => new Success(result);

    public static implicit operator Result<TSuccess, TError>(TError result)
        => new Failure(result);

    public static implicit operator Result<TSuccess, TError>(Result<TSuccess, Unit> result)
        => new Success(result.GetValue());

    public static implicit operator Result<TSuccess, TError>(Result<Unit, TError> result)
        => new Failure(result.GetError());
}