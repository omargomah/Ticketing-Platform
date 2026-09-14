using Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.ConfirmEmailCommand
{
    public sealed record ConfirmEmailCommand(string token, string userId) : IRequest<Result>;

}
