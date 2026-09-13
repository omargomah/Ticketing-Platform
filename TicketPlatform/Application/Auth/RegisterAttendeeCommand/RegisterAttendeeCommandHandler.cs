using Application.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.RegisterAttendeeCommand
{
    public class RegisterAttendeeCommandHandler : IRequestHandler<RegisterAttendeeCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public RegisterAttendeeCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public Task<bool> Handle(RegisterAttendeeCommand request, CancellationToken cancellationToken)
        {
        }
    }
}
