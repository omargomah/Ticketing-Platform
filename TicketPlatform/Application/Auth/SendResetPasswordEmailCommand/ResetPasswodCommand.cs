using Domain.Shared;
using MediatR;
namespace Application.Auth.SendResetPasswordEmailCommand
{
    public sealed record SendResetPasswordEmailCommand(string Email) : IRequest;
}
