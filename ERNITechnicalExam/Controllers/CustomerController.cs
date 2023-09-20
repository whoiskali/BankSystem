using Application.Extensions;
using Application.Interfaces;
using Application.UseCases.Account.Commands;
using Application.UseCases.Account.Queries;
using Application.UseCases.Transaction.Commands;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator Mediator;
        private readonly IApplicationDbContext ApplicationDbContext;
        private readonly ClaimsPrincipal ClaimsPrincipal;
        public CustomerController(IMediator _mediator, IApplicationDbContext applicationDbContext, ClaimsPrincipal claimsPrincipal)
        {
            Mediator = _mediator;
            ApplicationDbContext = applicationDbContext;
            ClaimsPrincipal = claimsPrincipal;
        }

        /// <summary>
        /// Registers a new customer.
        /// </summary>
        /// <param name="command">The details to register a customer.</param>
        /// <returns>The result of the registration operation.</returns>
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] CustomerRegistration.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                var result = await Mediator.Send(command);
                await tr.CommitAsync();
                return Ok(result);
            }
            catch (Exception)
            {
                await tr.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Transfers money from one account to another.
        /// </summary>
        /// <param name="command">The details to transfer money.</param>
        /// <returns>The result of the transfer operation.</returns>
        [Authorize]
        [HttpPost("TransferMoney")]
        public async Task<IActionResult> TransferMoney([FromBody] TransferMoney.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                var user = await ApplicationDbContext.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).SingleOrDefaultAsync(x => x.Id == ClaimsPrincipal.GetUserId());
                if (user == null)
                    throw new Exception("User not found");
                if (user.Customer == null)
                    throw new Exception("You don't have a Customer Account");
                if (user.Customer.Accounts.IsNullOrEmpty())
                    throw new Exception("You don't have any accounts");
                if (user.Customer.Accounts.SingleOrDefault(x => x.AccountNumber == command.SenderAccountNumber) == null)
                    throw new Exception("Account not found");

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

        /// <summary>
        /// Retrieves the account balance.
        /// </summary>
        /// <param name="AccountNumber">The account number for which to inquire about the balance.</param>
        /// <returns>The account balance.</returns>
        [Authorize]
        [HttpGet("BalanceInquiry/{AccountNumber}")]
        public async Task<IActionResult> BalanceInquiry(int AccountNumber)
        {
            var user = await ApplicationDbContext.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).SingleOrDefaultAsync(x => x.Id == ClaimsPrincipal.GetUserId());
            if (user == null)
                throw new Exception("User not found");
            if (user.Customer == null)
                throw new Exception("You don't have a Customer Account");
            if (user.Customer.Accounts.IsNullOrEmpty())
                throw new Exception("You don't have any accounts");
            if (user.Customer.Accounts.SingleOrDefault(x => x.AccountNumber == AccountNumber) == null)
                throw new Exception("Account not found");

            var query = new BalanceInquiry.Query(AccountNumber);
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                if (!ClaimsPrincipal.IsInRole(UserType.Teller.ToString()))
                    throw new UnauthorizedAccessException("No Permission to take this action.");
                var result = await Mediator.Send(query);
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
