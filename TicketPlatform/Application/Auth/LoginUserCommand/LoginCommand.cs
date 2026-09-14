using Domain.Shared;
using MediatR;
namespace Application.Auth.LoginUserCommand
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
