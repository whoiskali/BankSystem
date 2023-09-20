using Application.Interfaces;
using Application.UseCases.User.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Application.UseCases.User.Commands.Login;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator Mediator;
        public UserController(IMediator mediator)
        {
            Mediator = mediator;
        }
        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="command">The details to log in a user.</param>
        /// <returns>The result of the login operation.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login.Command command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
