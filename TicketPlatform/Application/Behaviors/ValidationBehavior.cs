using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Behaviors
{
    public sealed class ValidationBehaviors<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);
            ValidationContext<TRequest> validationContext = new ValidationContext<TRequest>(request);
            ValidationResult[] result = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
            List<ValidationFailure> validationFailures = result.SelectMany(vr => vr.Errors).Where(e => e is not null).ToList();
            if (validationFailures.Count != 0)
                throw new ValidationException(validationFailures);
            return await next(cancellationToken);

        }
    }

}
