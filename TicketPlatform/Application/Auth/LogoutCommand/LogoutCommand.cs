using MediatR;
namespace Application.Auth.LogoutCommand
{
    public sealed record LogoutCommand(string RefreshToken):IRequest;
}
