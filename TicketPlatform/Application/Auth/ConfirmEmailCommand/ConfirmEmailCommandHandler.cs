using Application.IServices;
using Domain.Shared;
using MediatR;
namespace Application.Auth.ConfirmEmailCommand
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ConfirmEmailCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            return await _identityService.ConfirmEmailAsync(command);
        }
    }
}
