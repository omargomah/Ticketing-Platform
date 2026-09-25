using Application.Auth.ConfirmEmailCommand;
using Application.Auth.LoginUserCommand;
using Application.Auth.LogoutCommand;
using Application.Auth.RefreshTokenCommand;
using Application.Auth.RegisterAttendeeCommand;
using Application.Auth.RegisterOrganizerCommand;
using Application.Auth.ResetPasswordCommand;
using Application.Auth.SendConfirmEmailCommand;
using Application.Auth.SendResetPasswordEmailCommand;
using Asp.Versioning;
using Domain.Entities;
using Domain.Shared;
using Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
     
        /// <summary>
        /// Registers a new attendee user account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1.0/Auth/register-attendee
        ///     {
        ///        "fName": "John",
        ///        "lName": "Doe",
        ///        "email": "john.doe@example.com",
        ///        "password": "Password123!",
        ///        "confirmPassword": "Password123!"
        ///     }
        ///
        /// </remarks>
        /// <param name="command">The attendee registration payload containing user details and credentials.</param>
        /// <param name="cancellationToken">Cancellation token to observe while executing the request.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created <see cref="Attendee"/> details on success.</returns>
        /// <response code="200">
        /// Attendee registered successfully.
        /// 
        /// Sample response:
        /// <code>
        /// {
        ///   "isSuccess": true,
        ///   "value": {
        ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "fName": "John",
        ///     "lName": "Doe",
        ///     "email": "john.doe@example.com"
        ///   }
        /// }
        /// </code>
        /// </response>
        /// <response code="400">
        /// Registration failed due to validation errors (e.g., invalid email, password mismatch, or email already exists).
        /// 
        /// Sample response:
        /// <code>
        /// {
        ///   "isSuccess": false,
        ///   "error": {
        ///     "code": "Auth.EmailAlreadyExists",
        ///     "message": "The provided email is already registered."
        ///   }
        /// }
        /// </code>
        /// </response>
        [HttpPost("register-attendee")]
        [ProducesResponseType(typeof(Result<Attendee>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<ActionResult<Result<Attendee>>> RegisterAttendee([FromBody] RegisterAttendeeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// Registers a new event organizer account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1.0/Auth/register-organizer
        ///     {
        ///        "email": "organizer@example.com",
        ///        "name": "Global Events Co",
        ///        "bankIban": "EG1234567890123456789012345",
        ///        "taxRegistrationNumber": "987654321",
        ///        "password": "Password123!",
        ///        "confirmPassword": "Password123!"
        ///     }
        ///
        /// </remarks>
        /// <param name="command">The organizer registration payload containing organization details, tax ID, IBAN, and credentials.</param>
        /// <param name="cancellationToken">Cancellation token to observe while executing the request.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created <see cref="Organizer"/> details on success.</returns>
        /// <response code="200">
        /// Organizer registered successfully.
        /// 
        /// Sample response:
        /// <code>
        /// {
        ///   "isSuccess": true,
        ///   "value": {
        ///     "id": "4ga95f64-5717-4562-b3fc-2c963f66afa7",
        ///     "name": "Global Events Co",
        ///     "email": "organizer@example.com",
        ///     "bankIban": "EG1234567890123456789012345",
        ///     "taxRegistrationNumber": "987654321"
        ///   }
        /// }
        /// </code>
        /// </response>
        /// <response code="400">
        /// Registration failed due to validation errors (e.g., invalid IBAN, invalid tax registration number, password mismatch, or duplicate email).
        /// 
        /// Sample response:
        /// <code>
        /// {
        ///   "isSuccess": false,
        ///   "error": {
        ///     "code": "Auth.InvalidRegistrationData",
        ///     "message": "Tax registration number is invalid or already in use."
        ///   }
        /// }
        /// </code>
        /// </response>
        [HttpPost("register-organizer")]          
        [ProducesResponseType(typeof(Result<Organizer>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<ActionResult<Result<Organizer>>> RegisterOrganizer([FromBody] RegisterOrganizerCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.IsSuccess)
                return Ok(result);
            
            return BadRequest(result);
        }

        /// <summary>
        /// Authenticates a user and issues access and refresh tokens.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1.0/Auth/login
        ///     {
        ///        "email": "user@example.com",
        ///        "password": "Password123!"
        ///     }
        ///
        /// Upon successful authentication, a secure HttpOnly cookie named "refreshToken" is attached to the response.
        /// </remarks>
        /// <param name="command">Login credentials containing Email and Password.</param>
        /// <param name="cancellationToken">Cancellation token to observe while executing the request.</param>
        /// <returns>A <see cref="Result{T}"/> containing access token and refresh token details on success.</returns>
        /// <response code="200">
        /// Authentication successful. Returns access token and sets HttpOnly refresh token cookie.
        /// 
        /// Sample response:
        /// <code>
        /// {
        ///   "isSuccess": true,
        ///   "value": {
        ///     "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
        ///   }
        /// }
        /// </code>
        /// </response>
        /// <response code="401">
        /// Authentication failed due to invalid credentials, unverified email, or locked account.
        /// 
        /// Sample response:
        /// <code>
        /// {
        ///   "isSuccess": false,
        ///   "error": {
        ///     "code": "Auth.InvalidCredentials",
        ///     "message": "Invalid email or password."
        ///   }
        /// }
        /// </code>
        /// </response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(Result<LoginResponse>), 200)]
        [ProducesResponseType(typeof(Result), 401)]
        public async Task<ActionResult<Result<LoginResponse>>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFail)
                return Unauthorized(result);

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

        /// <summary>
        /// Sends an email verification link to the specified user email address.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1.0/Auth/send-confirm-email
        ///     {
        ///        "email": "user@example.com"
        ///     }
        ///
        /// </remarks>
        /// <param name="command">Payload containing the recipient's email address.</param>
        /// <param name="cancellationToken">Cancellation token to observe while executing the request.</param>
        /// <returns>An empty <see cref="IActionResult"/> indicating that the request was processed.</returns>
        /// <response code="200">Confirmation email sent or queued successfully.</response>
        [HttpPost]
        [Route("send-confirm-email")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SendConfirmEmail([FromBody] SendConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("confirm-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFail)
                return BadRequest(result);

            return Ok(result);
        }
        
        [HttpPost]
        [Route("send-reset-password-email")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] SendResetPasswordEmailCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    
        [HttpPost]
        [Route("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<ActionResult<Result>> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            Result result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(typeof(Result<LoginResponse>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<ActionResult<Result<LoginResponse>>> Refresh()
        {
            string? refreshToken =  Request.Cookies["refreshToken"];
           if(string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized();
            Result<LoginResponse> refreshResult = await _mediator.Send(new RefreshTokenCommand(refreshToken));
        if(refreshResult.IsFail)
                return Unauthorized(refreshResult);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenExpireAfterDays)
            };
            Response.Cookies.Append("refreshToken", refreshResult.Value?.RefreshToken!, cookieOptions);
            return Ok(refreshResult);
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout()
        {
            string? providedToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(providedToken))
            {
                await _mediator.Send(new LogoutCommand(providedToken));
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }

            return Ok();
        }

    }
}
