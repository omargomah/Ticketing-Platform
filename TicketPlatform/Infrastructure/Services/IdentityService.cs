using Application.Auth.RegisterAttendeeCommand;
using Application.IServices;
using Domain.Shared;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class IdentityService: IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public IdentityService(UserManager<AppUser> userManager ,IConfiguration configuration , IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<bool> CheckEmailIsUniqueAsync(string email, CancellationToken token)
        {
            AppUser? user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        public async Task<Result> DeleteAppUserAsync(string userId , CancellationToken cancellationToken)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(Error.Create("User.NotFound", "User not found"));
            IdentityResult identityResult = await _userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
                return Result.Failure(Error.Create("User.DeletionFailed", string.Join(", ", identityResult.Errors.Select(e => e.Description))));
            return Result.Success();
        }
        public async Task<Result<string>> RegisterAsync(RegisterAttendeeCommand registerAttendeeCommand, CancellationToken cancellationToken)
        {
            AppUser user = new AppUser()
            {
                Email = registerAttendeeCommand.Email,
                UserName = registerAttendeeCommand.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerAttendeeCommand.Password);
            if (!result.Succeeded)
                return Result.Failure<string>(Error.Create("User.RegistrationFailed", string.Join(", ", result.Errors.Select(e => e.Description))));
            await SendEmailConfirmationMailAsync(user, cancellationToken);
            return Result.Success(user.Id.ToString()); 
        }
        private async Task SendEmailConfirmationMailAsync(AppUser user , CancellationToken cancellationToken)
        {
            string emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $@"{_configuration["AppUrl"]}/api/Auth/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmToken)}";
            await _emailService.SendConfirmEmailAsync(user.Email!, url, cancellationToken);
        }





    }
}
