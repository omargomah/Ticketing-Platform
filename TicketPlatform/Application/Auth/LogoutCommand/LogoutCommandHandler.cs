using Application.IServices;
using MediatR;
namespace Application.Auth.LogoutCommand
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IIdentityService _identityService;

        public LogoutCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _identityService.LogoutAsync(request.RefreshToken, cancellationToken);
        }
    }
}
