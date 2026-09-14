using Application.Auth.RegisterAttendeeCommand;
using Domain.Shared;

namespace Application.IServices
{
    public interface IIdentityService
    {
        Task<bool> CheckEmailIsUniqueAsync(string email, CancellationToken token);
        Task<Result<string>> RegisterAsync(RegisterAttendeeCommand registerAttendeeCommand, CancellationToken cancellationToken);
        Task<Result> DeleteAppUserAsync(string userId, CancellationToken cancellationToken);

    }
}
