namespace Application.Auth.LoginUserCommand
{
    public sealed record LoginResponse(bool IsSuccess, string? code = null ,string? Error = null, string? RefreshToken = null, string? AccessToken= null);   
}
