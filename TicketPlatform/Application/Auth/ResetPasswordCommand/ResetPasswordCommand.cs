using Domain.Shared;
using MediatR;

namespace Application.Auth.ResetPasswordCommand
{
    public sealed record ResetPasswordCommand(string UserId, string Token, string NewPassword , string ConfirmNewPassword) : IRequest<Result>;
}
