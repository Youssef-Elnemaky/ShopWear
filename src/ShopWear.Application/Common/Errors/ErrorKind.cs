using System.Text.Json.Serialization;

namespace ShopWear.Application.Common.Errors;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorKind
{
    Failure = 0,
    Unexpected = 1,
    Validation = 2,
    Conflict = 3,
    NotFound = 4,
    Unauthorized = 5,
    Forbidden = 6
}