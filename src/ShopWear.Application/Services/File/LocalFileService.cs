using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ShopWear.Application.Common.Errors;
using ShopWear.Application.Common.Results;

namespace ShopWear.Application.Services.Files;

public class LocalFileService : IFileService
{
    private readonly string _webRoot;
    private const string UploadRoot = "uploads";
    private static readonly HashSet<string> Img = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg",".jpeg",".png",".webp",".avif" };
    private static readonly HashSet<string> Doc = new(StringComparer.OrdinalIgnoreCase)
        { ".pdf",".doc",".docx",".xls",".xlsx",".ppt",".pptx",".txt" };
    private static readonly HashSet<string> Vid = new(StringComparer.OrdinalIgnoreCase)
        { ".mp4",".webm",".mov",".m4v" };
    public LocalFileService(IWebHostEnvironment env)
    {
        _webRoot = env.WebRootPath;
        Directory.CreateDirectory(Path.Combine(_webRoot, UploadRoot));
    }

    public async Task<Result<string>> SaveAsync(IFormFile file, string folder, FileKind kind, long? maxBytes = null)
    {
        if (file is null || file.Length == 0)
            return FileError.FileEmpty();

        if (maxBytes is long cap && file.Length > cap)
            return FileError.FileTooLarge(maxBytes.Value / (1024 * 1024));

        var ext = Path.GetExtension(file.FileName);
        if (!IsAllowed(ext, kind))
            return FileError.FileNotSupported(kind);

        var safeFolder = NormalizeFolder(folder);
        var dir = Path.Combine(_webRoot, safeFolder);
        var name = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(dir, name);

        try
        {
            Directory.CreateDirectory(dir);
            await using var dst = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(dst);
            var url = "/" + Path.Combine(safeFolder, name).Replace('\\', '/');
            return url;
        }
        catch (Exception ex)
        {
            return Error.Unexpected("File.SaveFailed", ex.Message);
        }
    }


    public async Task<Result<Success>> DeleteAsync(string url)
    {
        try
        {
            var rel = ToRelative(url);
            if (!string.IsNullOrWhiteSpace(rel))
            {
                var physical = Path.Combine(_webRoot, rel.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(physical)) File.Delete(physical);
            }
            return await Task.FromResult(ResultTypes.Success);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(Error.Failure("File.DeleteFailed", ex.Message));
        }
    }

    public async Task<Result<string>> UpdateAsync(IFormFile file, string oldUrlOrPath, string folder, FileKind kind, long? maxBytes = null)
    {
        var del = await DeleteAsync(oldUrlOrPath);
        if (del.IsError) return del.FirstError;
        return await SaveAsync(file, folder, kind, maxBytes);
    }

    private static bool IsAllowed(string ext, FileKind kind) => kind switch
    {
        FileKind.Image => Img.Contains(ext),
        FileKind.Document => Doc.Contains(ext),
        FileKind.Video => Vid.Contains(ext),
        _ => true
    };
    private static string NormalizeFolder(string? folder)
    => string.IsNullOrWhiteSpace(folder) ? "" : folder.Replace('\\', '/').Trim('/');

    private static string ToRelative(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        var s = input.Replace('\\', '/').Trim().TrimStart('/');
        return s;
    }
}