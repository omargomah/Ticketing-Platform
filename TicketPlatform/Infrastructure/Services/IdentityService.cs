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

        public async Task<Result> Register(RegisterAttendeeCommand registerAttendeeCommand, CancellationToken cancellationToken)
        {
            AppUser user = new AppUser()
            {
                Email = registerAttendeeCommand.Email,
                UserName = registerAttendeeCommand.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerAttendeeCommand.Password);
            if (!result.Succeeded)
                Result.Failure(Error.Create("User.RegistrationFailed", string.Join(", ", result.Errors.Select(e => e.Description))));
            string url = await GenerateEmailConfirmationUrl(user);
            await _emailService.SendConfirmEmailAsync(user.Email, url, cancellationToken);
            return Result.Success(user.Id); 
        }
        private async Task<string> GenerateEmailConfirmationUrl(AppUser user)
        {
            string emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return $@"{_configuration["AppUrl"]}/api/Auth/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmToken)}";
        }





    }
}
