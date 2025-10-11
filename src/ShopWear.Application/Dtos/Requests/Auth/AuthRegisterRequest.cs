namespace ShopWear.Application.Dtos.Requests.Auth;

public record AuthRegisterRequest(string FirstName, string LastName, string Email, string Password);