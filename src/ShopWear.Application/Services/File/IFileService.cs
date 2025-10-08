using Microsoft.AspNetCore.Http;
using ShopWear.Application.Common.Results;

namespace ShopWear.Application.Services.Files;

public interface IFileService
{
    Task<Result<string>> SaveAsync(IFormFile file, string folder, FileKind fileKind, long? maxBytes = null);
    Task<Result<Success>> DeleteAsync(string filePath);
    Task<Result<string>> UpdateAsync(IFormFile file, string oldUrlOrPath, string folder, FileKind kind, long? maxBytes = null);

}