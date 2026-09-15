using Application.Auth.LoginUserCommand;
using Application.IServices;
using Domain.Shared;
using MediatR;

namespace Application.Auth.RefreshTokenCommand
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
    {
        private readonly IIdentityService _identityService;

        public RefreshTokenCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return _identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        }
    }
}
