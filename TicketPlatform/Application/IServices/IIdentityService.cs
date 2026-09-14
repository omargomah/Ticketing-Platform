using Application.Auth.ConfirmEmailCommand;
using Application.Auth.LoginUserCommand;
using Application.Auth.RegisterAttendeeCommand;
using Domain.Enums;
using Domain.Shared;
namespace Application.IServices
{
    public interface IIdentityService
    {
        Task<bool> CheckEmailIsUniqueAsync(string email, CancellationToken token);
        Task<Result<string>> RegisterAsync(RegisterAttendeeCommand registerAttendeeCommand, UserRole Role ,CancellationToken cancellationToken);
        Task<Result> DeleteAppUserAsync(string userId, CancellationToken cancellationToken);
        Task<Result<LoginResponse>> LoginUserAsync(LoginCommand loginRequest);
        Task<Result> ConfirmEmailAsync(ConfirmEmailCommand command);

    }
}
