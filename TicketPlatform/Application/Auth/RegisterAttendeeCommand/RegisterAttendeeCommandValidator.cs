using Application.IServices;
using Domain.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.RegisterAttendeeCommand
{
    public class RegisterAttendeeCommandValidator : AbstractValidator<RegisterAttendeeCommand>
    {
        public RegisterAttendeeCommandValidator(IIdentityService identityService)
        {
            RuleFor(x => x.FName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(Constants.Attendee.FirstNameMaxLength).WithMessage("First name cannot exceed 50 characters.");
            
            RuleFor(x => x.LName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(Constants.Attendee.LastNameMaxLength).WithMessage("Last name cannot exceed 50 characters.");

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
        
        }

    }
}
