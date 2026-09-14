using Application.IServices;
using Domain.Shared;
using MediatR;

namespace Application.Auth.SendResetPasswordEmailCommand
{
    public class SendResetPasswordEmailCommandHandler : IRequestHandler<SendResetPasswordEmailCommand>
    {
        private readonly IIdentityService _identityService;

        public SendResetPasswordEmailCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task Handle(SendResetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            await _identityService.SendResetPasswordEmailAsync(request.Email,cancellationToken);
        }
    }
}
