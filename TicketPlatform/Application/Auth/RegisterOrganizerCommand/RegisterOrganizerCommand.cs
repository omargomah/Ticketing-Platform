using Domain.Shared;
using MediatR;
namespace Application.Auth.RegisterOrganizerCommand
{
    public sealed record RegisterOrganizerCommand(string Email,string Name ,string BankIban, string TaxRegistrationNumber, string Password, string ConfirmPassword) : IRequest<Result>;
}
