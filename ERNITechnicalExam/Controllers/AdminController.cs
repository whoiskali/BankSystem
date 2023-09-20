using Application.Interfaces;
using Application.UseCases.Account.Commands;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator Mediator;
        private readonly IApplicationDbContext ApplicationDbContext;
        private readonly ClaimsPrincipal ClaimsPrincipal;
        public AdminController(IMediator _mediator, IApplicationDbContext applicationDbContext, ClaimsPrincipal claimsPrincipal)
        {
            Mediator = _mediator;
            ApplicationDbContext = applicationDbContext;
            ClaimsPrincipal = claimsPrincipal;
        }

        /// <summary>
        /// Add a new teller to the system.
        /// </summary>
        /// <param name="command">The details to add a teller.</param>
        /// <returns>The result of the add operation.</returns>
        [Authorize]
        [HttpPost("AddTeller")]
        public async Task<IActionResult> AddTeller([FromBody] AddTeller.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                if (!ClaimsPrincipal.IsInRole(UserType.Admin.ToString()))
                    throw new UnauthorizedAccessException("No Permission to take this action.");
                var result = await Mediator.Send(command);
                await tr.CommitAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                await tr.RollbackAsync();
                return BadRequest(e.Message);
            }
        }
    }
}
