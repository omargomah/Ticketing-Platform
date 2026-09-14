using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Auth.RegisterOrganizerCommand
{
    public class RegisterOrganizerCommandHandler : IRequestHandler<RegisterOrganizerCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RegisterOrganizerCommandHandler> _logger;

        public RegisterOrganizerCommandHandler(IIdentityService identityService ,ILogger<RegisterOrganizerCommandHandler> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }
        public async Task<Result> Handle(RegisterOrganizerCommand request, CancellationToken cancellationToken)
        {
            Result<TaxRegistrationNumber> taxRegistrationNumberCreateResult = TaxRegistrationNumber.Create(request.TaxRegistrationNumber);
            if (taxRegistrationNumberCreateResult.IsFail)
                return taxRegistrationNumberCreateResult;
            
            Result<BankIban> BankIbanResult = BankIban.Create(request.BankIban);
            if (BankIbanResult.IsFail)
                return BankIbanResult;


            // create identity user
            Result<string> createAppUserResult = await _identityService.RegisterAsync(request.Email, request.Password, UserRole.Organizer, cancellationToken);
            if(createAppUserResult.IsFail)
                return createAppUserResult;
            
            //create organizer
            Result organizerResult = Organizer.Create(request.Name, taxRegistrationNumberCreateResult.Value!, BankIbanResult.Value!);

            if (organizerResult.IsSuccess)
                return organizerResult;

            // if organizer creation fails, delete the created identity user
            _logger.LogError(organizerResult.Error.Message);
            Result deleteResult = await _identityService.DeleteAppUserAsync(createAppUserResult.Value!, cancellationToken);
            if(deleteResult.IsFail)
                _logger.LogCritical(deleteResult.Error.Message);
            return deleteResult;
        }
    }
}
