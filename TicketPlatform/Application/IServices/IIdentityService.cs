using Application.Auth.ConfirmEmailCommand;
using Application.Auth.LoginUserCommand;
using Application.Auth.RegisterAttendeeCommand;
using Application.Auth.RegisterOrganizerCommand;
using Application.Auth.ResetPasswordCommand;
using Application.Auth.SendConfirmEmailCommand;
using Domain.Enums;
using Domain.Shared;
namespace Application.IServices
{
    public interface IIdentityService
    {
        Task<bool> CheckEmailIsUniqueAsync(string email, CancellationToken token);
        Task<Result<string>> RegisterAsync(string email, string password, UserRole Role ,CancellationToken cancellationToken);
        Task<Result> DeleteAppUserAsync(string userId, CancellationToken cancellationToken);
        Task<Result<LoginResponse>> LoginUserAsync(LoginCommand loginRequest);
        Task<Result> ConfirmEmailAsync(ConfirmEmailCommand command);
        Task SendResetPasswordEmailAsync(string email, CancellationToken cancellationToken);
        Task<Result> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellation);
        Task SendConfirmEmailAsync(SendConfirmEmailCommand command, CancellationToken cancellationToken);

    }
}
