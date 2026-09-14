using Application.IServices;
using Domain.Entities;
using Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.RegisterAttendeeCommand
{
    public class RegisterAttendeeCommandHandler : IRequestHandler<RegisterAttendeeCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RegisterAttendeeCommandHandler> _logger;

        public RegisterAttendeeCommandHandler(IIdentityService identityService ,ILogger<RegisterAttendeeCommandHandler> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }
        public async Task<Result> Handle(RegisterAttendeeCommand request, CancellationToken cancellationToken)
        {
            // create identity user
            Result<string> createAppUserResult = await _identityService.RegisterAsync(request,cancellationToken);
            if(!createAppUserResult.IsSuccess)
                return createAppUserResult;
            
            //create attendee
            Result attendeeResult = Attendee.Create(request.FName, request.LName);
            if(attendeeResult.IsSuccess)
                return attendeeResult;

            // if attendee creation fails, delete the created identity user
            _logger.LogError(attendeeResult.Error.Message);
            Result deleteResult = await _identityService.DeleteAppUserAsync(createAppUserResult.Value!, cancellationToken);
            if(deleteResult.IsFail)
                _logger.LogCritical(deleteResult.Error.Message);
            return deleteResult;
        }
    }
}
