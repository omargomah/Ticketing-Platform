using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;
using Domain.Shared;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Auth.RegisterOrganizerCommand
{
    public class RegisterOrganizerCommandHandler : IRequestHandler<RegisterOrganizerCommand, Result<Organizer>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<RegisterOrganizerCommandHandler> _logger;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterOrganizerCommandHandler(IIdentityService identityService ,
            ILogger<RegisterOrganizerCommandHandler> logger,
            IOrganizerRepository organizerRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _logger = logger;
            _organizerRepository = organizerRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Organizer>> Handle(RegisterOrganizerCommand request, CancellationToken cancellationToken)
        {
            Result<TaxRegistrationNumber> taxRegistrationNumberCreateResult = TaxRegistrationNumber.Create(request.TaxRegistrationNumber);
            if (taxRegistrationNumberCreateResult.IsFail)
                return Result.Failure<Organizer>(taxRegistrationNumberCreateResult.Error!);
            
            Result<BankIban> BankIbanResult = BankIban.Create(request.BankIban);
            if (BankIbanResult.IsFail)
                return Result.Failure<Organizer>(BankIbanResult.Error!);


            // create identity user
            Result<string> createAppUserResult = await _identityService.RegisterAsync(request.Email, request.Password, UserRole.Organizer, cancellationToken);
            if(createAppUserResult.IsFail)
                return Result.Failure<Organizer>(createAppUserResult.Error!);
            
            //create organizer
            Result<Organizer> organizerResult = Organizer.Create(request.Name, taxRegistrationNumberCreateResult.Value!, BankIbanResult.Value!);

            if (organizerResult.IsSuccess)
            {
                await _organizerRepository.AddAsync(organizerResult.Value!);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return organizerResult;                
            }


            // if organizer creation fails, delete the created identity user
            _logger.LogError(organizerResult.Error.Message);
            Result deleteResult = await _identityService.DeleteAppUserAsync(createAppUserResult.Value!, cancellationToken);
            if(deleteResult.IsFail)
                _logger.LogCritical(deleteResult.Error.Message);
            return Result.Failure<Organizer>(deleteResult.Error!);
        }
    }
}
