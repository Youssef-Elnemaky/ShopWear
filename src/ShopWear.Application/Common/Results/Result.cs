using ShopWear.Application.Common.Errors;

namespace ShopWear.Application.Common.Results;

public readonly record struct Success;
public readonly record struct Updated;
public readonly record struct Created;
public readonly record struct Deleted;

public static class ResultTypes
{
    public static Success Success => default;
    public static Created Created => default;
    public static Updated Updated => default;
    public static Deleted Deleted => default;
}

public sealed class Result<TValue> : IResult<TValue>
{
    private readonly TValue _value = default!;
    private readonly IReadOnlyList<Error> _errors = Array.Empty<Error>();

    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;

    public TValue Value =>
        IsSuccess ? _value : throw new InvalidOperationException("Cannot access Value when IsSuccess is false.");

    public IReadOnlyList<Error> Errors =>
        IsError ? _errors : throw new InvalidOperationException("Cannot access Errors when IsError is false.");

    public Error FirstError =>
        _errors.Count > 0 ? _errors[0] : throw new InvalidOperationException("No errors available.");

    private Result(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        _value = value;
        _errors = Array.Empty<Error>();
        IsSuccess = true;
    }

    private Result(Error error)
    {
        _errors = new[] { error };
        IsSuccess = false;
    }

    private Result(IReadOnlyList<Error> errors)
    {
        if (errors is null || errors.Count == 0)
            throw new ArgumentException("Provide at least one error.", nameof(errors));

        _errors = errors.ToArray(); // defensive copy
        IsSuccess = false;
    }

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<TValue> Failure(Error error) => new(error);

    public static Result<TValue> Failure(params Error[] errors) => new(errors);

    public static implicit operator Result<TValue>(Error error) => new(error);

    public static implicit operator Result<TValue>(Error[] errors) => new(errors);

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public TNext Match<TNext>(
        Func<TValue, TNext> onValue,
        Func<IReadOnlyList<Error>, TNext> onError) =>
        IsSuccess ? onValue(_value) : onError(_errors);
}
