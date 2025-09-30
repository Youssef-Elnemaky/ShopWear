using ShopWear.Application.Common.Errors;

namespace ShopWear.Application.Common.Results;

public interface IResult<out TValue>
{
    bool IsSuccess { get; }
    bool IsError { get; }
    TValue Value { get; }
    IReadOnlyList<Error> Errors { get; }

}