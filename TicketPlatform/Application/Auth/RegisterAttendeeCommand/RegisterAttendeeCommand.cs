using Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.RegisterAttendeeCommand
{
    public sealed record RegisterAttendeeCommand(string FName ,string LName,string Email, string Password, string ConfirmPassword) : IRequest<Result>;
}
