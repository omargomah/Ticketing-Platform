using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.SendResetPasswordEmailCommand
{
    public class SendResetPasswordEmailCommandValidator : AbstractValidator<SendResetPasswordEmailCommand>
    {
        public SendResetPasswordEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
