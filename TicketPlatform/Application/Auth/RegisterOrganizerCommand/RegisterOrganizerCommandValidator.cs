using Application.IServices;
using Domain.Shared;
using FluentValidation;
namespace Application.Auth.RegisterOrganizerCommand
{
    public class RegisterOrganizerCommandValidator : AbstractValidator<RegisterOrganizerCommand>
    {
        public RegisterOrganizerCommandValidator(IIdentityService identityService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(Constants.Organizer.NameMaxLength).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.").MustAsync(identityService.CheckEmailIsUniqueAsync);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
            
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .MinimumLength(8).WithMessage("Password confirmation must be at least 8 characters long.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.BankIban)
                .NotEmpty().WithMessage("Bank IBAN is required.")
                .MaximumLength(Constants.BankIban.RequiredLength).WithMessage($"Bank IBAN cannot exceed {Constants.BankIban.RequiredLength} characters.");
            
            RuleFor(x => x.TaxRegistrationNumber)
                .NotEmpty().WithMessage("Tax Registration Number is required.")
                .MaximumLength(Constants.TaxRegistrationNumber.RequiredLength).WithMessage($"Tax Registration Number cannot exceed {Constants.TaxRegistrationNumber.RequiredLength} characters.");

        }

    }
}
