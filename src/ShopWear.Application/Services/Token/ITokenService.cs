using ShopWear.Application.Common.Results;
using ShopWear.Application.Dtos.Requests.Auth;
using ShopWear.Application.Dtos.Responses.Auth;
using ShopWear.DataAccess.Models.Auth;

namespace ShopWear.Application.Services.Token;


public interface ITokenService
{
    Task<Result<AuthResponse>> GenerateTokenAsync(ApplicationUser user, IList<string> roles);
    Task<Result<AuthResponse>> RefreshAsync(AuthRefreshRequest request);
}