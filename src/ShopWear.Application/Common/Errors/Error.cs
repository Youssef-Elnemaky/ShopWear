namespace ShopWear.Application.Common.Errors;

public readonly record struct Error
{
    public String Code { get; }
    public String Description { get; }
    public ErrorKind Kind { get; }

    private Error(string code, string description, ErrorKind kind)
    {
        Code = code;
        Description = description;
        Kind = kind;
    }
    public static Error Failure(string code = nameof(Failure), string description = "General Failure.") =>
        new(code, description, ErrorKind.Failure);
    public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected Error.") =>
        new(code, description, ErrorKind.Unexpected);
    public static Error Validation(string code = nameof(Validation), string description = "Validation Error.") =>
        new(code, description, ErrorKind.Validation);
    public static Error Conflict(string code = nameof(Conflict), string description = "Conflict Error.") =>
        new(code, description, ErrorKind.Conflict);
    public static Error NotFound(string code = nameof(NotFound), string description = "NotFound Error.") =>
        new(code, description, ErrorKind.NotFound);
    public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized Error.") =>
        new(code, description, ErrorKind.Unauthorized);
    public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden Error.") =>
        new(code, description, ErrorKind.Forbidden);
}