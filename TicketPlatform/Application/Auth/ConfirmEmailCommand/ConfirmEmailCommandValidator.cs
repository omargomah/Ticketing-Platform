using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.ConfirmEmailCommand
{
    public class ConfirmEmailCommandValidator:AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.token).NotEmpty().WithMessage("Token is required.");
        }
    }
}
