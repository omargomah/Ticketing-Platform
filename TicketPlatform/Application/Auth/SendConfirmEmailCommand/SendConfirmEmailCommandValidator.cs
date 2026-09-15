using FluentValidation;

namespace Application.Auth.SendConfirmEmailCommand
{
    public class SendConfirmEmailCommandValidator :AbstractValidator<SendConfirmEmailCommand>
    {
        public SendConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
