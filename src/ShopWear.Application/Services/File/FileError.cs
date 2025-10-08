using ShopWear.Application.Common.Errors;

namespace ShopWear.Application.Services.Files;

public static class FileError
{
    public static Error FileEmpty()
        => Error.Validation("File.Size.Empty", "File is empty");
    public static Error FileTooLarge(long maxSize)
        => Error.Validation("File.TooLarge", $"File size exceeds max size of {maxSize} MB.");

    public static Error FileNotSupported(FileKind supported)
        => Error.Validation("File.FileKindNotSupported", $"Unsupported format type of {supported}");
}