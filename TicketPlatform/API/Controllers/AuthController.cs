using Application.Auth.RegisterAttendeeCommand;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
     
        [HttpPost("register-attendee")]
        /// <summary>
        /// Registers a new attendee
        /// </summary>
        /// <param name="command">The command containing attendee information</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the registration</returns>
        public async Task<IActionResult> RegisterAttendee([FromBody] RegisterAttendeeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }



    }
}
