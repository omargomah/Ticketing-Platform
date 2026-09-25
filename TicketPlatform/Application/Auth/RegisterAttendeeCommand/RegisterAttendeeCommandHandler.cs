using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;
using Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.RegisterAttendeeCommand
{
    public class RegisterAttendeeCommandHandler : IRequestHandler<RegisterAttendeeCommand, Result<Attendee>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RegisterAttendeeCommandHandler> _logger;
        private readonly IAttendeeRepository _attendeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterAttendeeCommandHandler(IIdentityService identityService ,
            ILogger<RegisterAttendeeCommandHandler> logger ,
            IAttendeeRepository attendeeRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _logger = logger;
            _attendeeRepository = attendeeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Attendee>> Handle(RegisterAttendeeCommand request, CancellationToken cancellationToken)
        {
            // create identity user
            Result<string> createAppUserResult = await _identityService.RegisterAsync(request.Email,request.Password, UserRole.Attendee, cancellationToken);
            if(createAppUserResult.IsFail)
                return Result.Failure<Attendee>(createAppUserResult.Error!);
            
            //create attendee
            Result<Attendee> attendeeResult = Attendee.Create(request.FName, request.LName);
            if (attendeeResult.IsSuccess)
            {
                await _attendeeRepository.AddAsync(attendeeResult.Value!);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return attendeeResult;
            }

            // if attendee creation fails, delete the created identity user
            _logger.LogError(attendeeResult.Error.Message);
            Result deleteResult = await _identityService.DeleteAppUserAsync(createAppUserResult.Value!, cancellationToken);
            if(deleteResult.IsFail)
                _logger.LogCritical(deleteResult.Error.Message);
            return Result.Failure<Attendee>(deleteResult.Error!);
        }
    }
}
