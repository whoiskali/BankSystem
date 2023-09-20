using Application.Interfaces;
using Application.UseCases.Account.Commands;
using Application.UseCases.Account.Queries;
using Application.UseCases.Customer.Commands;
using Application.UseCases.Transaction.Commands;
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
    public class TellerController : ControllerBase
    {
        private readonly IMediator Mediator;
        private readonly IApplicationDbContext ApplicationDbContext;
        private readonly ClaimsPrincipal ClaimsPrincipal;
        public TellerController(IMediator _mediator, IApplicationDbContext applicationDbContext, ClaimsPrincipal claimsPrincipal)
        {
            Mediator = _mediator;
            ApplicationDbContext = applicationDbContext;
            ClaimsPrincipal = claimsPrincipal;
        }

        /// <summary>
        /// Deposits money into an account.
        /// </summary>
        /// <param name="command">The command to deposit money.</param>
        /// <returns>The result of the deposit operation.</returns>
        [Authorize]
        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] Deposit.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                if (!ClaimsPrincipal.IsInRole(UserType.Teller.ToString()))
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
                return BadRequest(e.InnerException?.ToString() ?? e.Message);
            }
        }

        /// <summary>
        /// Adds a new customer to the system.
        /// </summary>
        /// <param name="command">The details to add a customer.</param>
        /// <returns>The result of the customer addition operation.</returns>
        [Authorize]
        [HttpPost("AddCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomer.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                if (!ClaimsPrincipal.IsInRole(UserType.Teller.ToString()))
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

        /// <summary>
        /// Opens a new bank account.
        /// </summary>
        /// <param name="command">The details to open a new account.</param>
        /// <returns>The result of the account opening operation.</returns>
        [Authorize]
        [HttpPost("OpenAccount")]
        public async Task<IActionResult> OpenAccount([FromBody] OpenAccount.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                if (!ClaimsPrincipal.IsInRole(UserType.Teller.ToString()))
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
                if (!ClaimsPrincipal.IsInRole(UserType.Teller.ToString()))
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

        /// <summary>
        /// Withdraws cash from an account.
        /// </summary>
        /// <param name="command">The details to withdraw cash.</param>
        /// <returns>The result of the cash withdrawal operation.</returns>
        [Authorize]
        [HttpPost("WithdrawCash")]
        public async Task<IActionResult> WithdrawCash([FromBody] WithdrawCash.Command command)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                if (!ClaimsPrincipal.IsInRole(UserType.Teller.ToString()))
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
        
        /// <summary>
         /// Retrieves the account balance.
         /// </summary>
         /// <param name="AccountNumber">The account number for which to inquire about the balance.</param>
         /// <returns>The account balance.</returns>
        [Authorize]
        [HttpGet("BalanceInquiry/{AccountNumber}")]
        public async Task<IActionResult> BalanceInquiry(int AccountNumber)
        {
            var tr = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                var query = new BalanceInquiry.Query(AccountNumber);
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
