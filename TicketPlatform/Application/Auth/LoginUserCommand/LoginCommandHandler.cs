using Application.IServices;
using Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.LoginUserCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.LoginUserAsync(request);
        }
    }
}
