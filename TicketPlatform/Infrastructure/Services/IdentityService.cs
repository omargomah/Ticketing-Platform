using Application.Auth.ConfirmEmailCommand;
using Application.Auth.LoginUserCommand;
using Application.Auth.RegisterAttendeeCommand;
using Application.Auth.RegisterOrganizerCommand;
using Application.Auth.ResetPasswordCommand;
using Application.Auth.SendConfirmEmailCommand;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;
using Domain.Shared;
using Infrastructure.Identity;
using Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class IdentityService: IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IOptionsSnapshot<JwtOptions> _jwtConfiguration;
        private readonly ILogger<IdentityService> _logger;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public IdentityService(UserManager<AppUser> userManager ,
            IConfiguration configuration ,
            IEmailService emailService ,
            IOptionsSnapshot<JwtOptions> jwtConfiguration , 
            ILogger<IdentityService> logger,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _jwtConfiguration = jwtConfiguration;
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
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

        #region Register
        public async Task<Result<string>> RegisterAsync(string email , string password , UserRole Role, CancellationToken cancellationToken)
        {
            AppUser user = new AppUser()
            {
                Email = email,
                UserName = email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result.Failure<string>(Error.Create("User.RegistrationFailed", string.Join(", ", result.Errors.Select(e => e.Description))));
            
            
            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, Role.ToString());

            if (!addRoleResult.Succeeded)
            {
                _logger.LogCritical(string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                await DeleteAppUserAsync(user.Id.ToString(), cancellationToken);
                return Result.Failure<string>(Error.Create("User.RegistrationFailed", string.Join(", ", addRoleResult.Errors.Select(e => e.Description))));
            }

            await SendConfirmEmailAsync(user, cancellationToken);
            return Result.Success(user.Id.ToString()); 
        }
        #endregion

        #region Login

        public async Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            RefreshToken? refreshTokeWillRevoked=  await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken, cancellationToken);    
        
            if(refreshTokeWillRevoked is null)
                return Result.Failure<LoginResponse>(Error.Create("User.RefreshTokenNotFound", "Refresh Token Not Found"));
            
            if (refreshTokeWillRevoked.IsExpired && refreshTokeWillRevoked.RevokedOn is not null)
            {
                refreshTokeWillRevoked.Revoke();
                _refreshTokenRepository.Update(refreshTokeWillRevoked);
                await _unitOfWork.SaveChangesAsync();
                                                                                       
                return Result.Failure<LoginResponse>(Error.Create("User.RefreshTokenRevoked", "Refresh Token Revoked"));
            }
            
            AppUser user = (await _userManager.FindByIdAsync(refreshTokeWillRevoked.UserId.ToString()))!;
            if (refreshTokeWillRevoked.IsActive)
            {

                RefreshToken newRefreshToken = RefreshToken.Create(user.Id, DateTime.UtcNow.AddDays(_jwtConfiguration.Value.RefreshTokenExpireAfterDays));
                try
                {
                    refreshTokeWillRevoked.Revoke();
                    _refreshTokenRepository.Update(refreshTokeWillRevoked);
                    await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "An error occurred while refreshing the token.");
                    throw;
                }
                return Result.Success(new LoginResponse(true, null!, newRefreshToken.Token, await GenerateAccessTokenAsync(user)));
            }

            await _emailService.SendWarningEmailThatRefreshTokenStealAsync(user.Email!,cancellationToken);
            return Result.Failure<LoginResponse>(Error.Create("User.RefreshTokenExpired", "Refresh Token Expired"));
        }
        public async Task<Result<LoginResponse>> LoginUserAsync(LoginCommand loginRequest)
        {
            AppUser? user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
                return Result.Failure<LoginResponse>(Error.Create("User.LoginFailed", "The Email or Password is Invalid"));

            bool isTheCorrectPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!isTheCorrectPassword)
                return Result.Failure<LoginResponse>(Error.Create("User.LoginFailed", "The Email or Password is Invalid"));

            if (_userManager.Options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                return Result.Failure<LoginResponse>(Error.Create("User.EmailNotConfirmed", "You Need to Confirm your Email"));

            RefreshToken refreshToken = RefreshToken.Create(user.Id,DateTime.UtcNow.AddDays(_jwtConfiguration.Value.RefreshTokenExpireAfterDays));

            return Result.Success(new LoginResponse(true, null! , refreshToken.Token , await GenerateAccessTokenAsync(user)));
        }
        private string GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan expiresIn)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.Value.Issuer,
                audience: _jwtConfiguration.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expiresIn),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            string userRole = (await _userManager.GetRolesAsync(user)).First();
            IEnumerable<Claim> claims =
                    [
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Email,user.Email!),
                        new Claim(ClaimTypes.Role, userRole),
                        new Claim("TokenType","Access"),
                    ];

            return GenerateJwtToken(claims, TimeSpan.FromMinutes(_jwtConfiguration.Value.AccessTokenExpireAfterMinutes));
        }
        //private string GenerateRefreshToken(AppUser user)
        //{
        //    IEnumerable<Claim> claims =
        //            [
        //                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
        //                new Claim("TokenType","Refresh"),
        //            ];
        //    return GenerateJwtToken(claims, TimeSpan.FromDays(_jwtConfiguration.Value.RefreshTokenExpireAfterDays));
        //}


        #endregion

        #region Email Confirmation
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailCommand command)
        {
            AppUser? user = await _userManager.FindByIdAsync(command.userId);

            if (user is null)
                return Result.Failure(Error.Create("User.NotFound", "User not found"));

            IdentityResult confirmEmailResult = await _userManager.ConfirmEmailAsync(user, command.token);

            if (!confirmEmailResult.Succeeded)
                return Result.Failure(Error.Create("User.ConfirmEmailFailed", "Failed to confirm email"));
                                                                                                          
            return Result.Success();
        }
        public async Task SendConfirmEmailAsync(SendConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            AppUser? user = await _userManager.FindByEmailAsync(command.Email);
            if (user is null)
                return;
            await SendConfirmEmailAsync(user, cancellationToken);
        }
        private async Task SendConfirmEmailAsync(AppUser user , CancellationToken cancellationToken)
        {
            // i don't add the version in url take care if not check it ,but it should take the default value of version that is v1
            string emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $@"{_configuration["AppUrl"]}/api/Auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmToken)}";
            await _emailService.SendConfirmEmailAsync(user.Email!, url, cancellationToken);
        }
        #endregion

        #region Reset Password
        public async Task SendResetPasswordEmailAsync(string email, CancellationToken cancellationToken)
        {
            AppUser? user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return;
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string resetPasswordToken = Uri.EscapeDataString(token);
            string url = $"{_configuration["FrontUrl"]}/Auth/reset-password?userId={user.Id}&token={Uri.EscapeDataString(resetPasswordToken)}";
            await _emailService.SendResetPasswordEmailAsync(user.Email!, url, cancellationToken);
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellation)
        {
            AppUser? user = await _userManager.FindByIdAsync(command.UserId);
            if (user is null)
                return Result.Failure(Error.Create("User.NotFound", "User not found"));
            IdentityResult result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
            if (!result.Succeeded)
                return  Result.Failure(Error.Create("User.ResetPasswordFailed", string.Join(", ", result.Errors.Select(e => e.Description))));
            await _refreshTokenRepository.DeleteRefreshTokensByUserIdAsync(user.Id);
            return  Result.Success() ;
        }
        #endregion

        #region Logout
        public async Task LogoutAsync(string refreshToken , CancellationToken cancellationToken)
        {
            RefreshToken? token = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken, cancellationToken);
            if (token is not null)
            {
                token.Revoke();
                _refreshTokenRepository.Update(token);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        
        #endregion
    }
}
