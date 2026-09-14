using Application.Auth.ConfirmEmailCommand;
using Application.Auth.LoginUserCommand;
using Application.Auth.RegisterAttendeeCommand;
using Application.Auth.RegisterOrganizerCommand;
using Application.Auth.ResetPasswordCommand;
using Application.Auth.SendConfirmEmailCommand;
using Application.Auth.SendResetPasswordEmailCommand;
using Asp.Versioning;
using Azure.Core;
using Domain.Shared;
using Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOptionsSnapshot<JwtOptions> _jwtOptions;

        public AuthController(IMediator mediator,IOptionsSnapshot<JwtOptions> jwtOptions)
        {
            _mediator = mediator;
            _jwtOptions = jwtOptions;
        }
     
        [HttpPost("register-attendee")]
        /// <summary>
        /// Registers a new attendee
        /// </summary>
        /// <param name="command">The command containing attendee information</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the registration</returns>
        public async Task<ActionResult<Result>> RegisterAttendee([FromBody] RegisterAttendeeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("register-organizer")]
        public async Task<ActionResult<Result>> RegisterOrganizer([FromBody] RegisterOrganizerCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.IsSuccess)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFail)
                return BadRequest(result);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenExpireAfterDays)
            };

            Response.Cookies.Append("refreshToken", result.Value?.RefreshToken!, cookieOptions);

            return Ok(result);
        }

        [HttpPost]
        [Route("send-confirm-email")]
        public async Task<IActionResult> SendConfirmEmail([FromBody] SendConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFail)
                return BadRequest(result);

            return Ok(result);
        }
        
        [HttpPost]
        [Route("send-reset-password-email")]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] SendResetPasswordEmailCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    
        [HttpPost]
        [Route("reset-password")]
        public async Task<ActionResult<Result>> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            Result result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

    }
}
