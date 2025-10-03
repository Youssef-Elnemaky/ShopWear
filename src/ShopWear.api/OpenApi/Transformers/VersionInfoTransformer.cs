using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ShopWear.api.OpenApi.Transformers;

internal class VersionInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var version = context.DocumentName;

        document.Info.Version = version;
        document.Info.Title = $"ShopWear API {version}";

        return Task.CompletedTask;
    }
}