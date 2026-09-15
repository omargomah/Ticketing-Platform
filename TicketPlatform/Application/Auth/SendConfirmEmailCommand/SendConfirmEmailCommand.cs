using MediatR;

namespace Application.Auth.SendConfirmEmailCommand
{
    public sealed record SendConfirmEmailCommand(string Email):IRequest;
}
