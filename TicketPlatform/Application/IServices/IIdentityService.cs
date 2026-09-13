using Application.Auth.RegisterAttendeeCommand;
using Domain.Shared;

namespace Application.IServices
{
    public interface IIdentityService
    {
        Task<bool> CheckEmailIsUniqueAsync(string email, CancellationToken token);
        Task<Result> Register(RegisterAttendeeCommand registerAttendeeCommand, CancellationToken cancellationToken);

    }
}
