using Application.Auth.LoginUserCommand;
using Domain.Shared;
using MediatR;
namespace Application.Auth.RefreshTokenCommand
{
    public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
}
