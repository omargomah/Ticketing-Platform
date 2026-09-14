using Application.IServices;
using Domain.Shared;
using MediatR;

namespace Application.Auth.ResetPasswordCommand
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return _identityService.ResetPasswordAsync(request, cancellationToken);
        }
    }
}
