using Application.IServices;
using MediatR;

namespace Application.Auth.SendConfirmEmailCommand
{
    public class SendConfirmEmailHandler : IRequestHandler<SendConfirmEmailCommand>
    {
        private readonly IIdentityService _identityService;

        public SendConfirmEmailHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
             await _identityService.SendConfirmEmailAsync(request, cancellationToken);
        }
    }
}
