using Application.Interfaces;
using Application.UseCases.Account.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Transaction.Commands
{
    public static class WithdrawCash
    {
        public record Command(int AccountNumber, decimal Amount) : IRequest<Result>;
        public record Result(decimal AvailableBalance);
        public record Handler(IApplicationDbContext ApplicationDbContext) : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    // Retrieve the account based on the account number.
                    var account = await ApplicationDbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == command.AccountNumber);

                    // Check if the account exists.
                    if (account == null) throw new Exception("Account not found.");

                    // Query the available balance of the account.
                    var balanceQuery = new BalanceInquiry.Query(account.AccountNumber);
                    var availableBalanceHandler = new BalanceInquiry.Handler(ApplicationDbContext);
                    var senderAvailableBalance = (await availableBalanceHandler.Handle(balanceQuery, cancellationToken)).Amount;

                    // Check if there is sufficient balance for the withdrawal.
                    if ((senderAvailableBalance - command.Amount) < 0)
                        throw new Exception("Insufficient balance.");

                    // Create a new transaction for the withdrawal.
                    var transaction = new Domain.Entities.Transaction()
                    {
                        Type = TransactionType.Withdraw,
                        Amount = command.Amount,
                        ReceiverAccount = null,
                        SenderAccount = account
                    };

                    // Add the transaction to the database.
                    var value = ApplicationDbContext.Transactions.Add(transaction);

                    // Save changes to the database.
                    await ApplicationDbContext.SaveChangesAsync(cancellationToken);

                    // Query the updated balance of the account.
                    var senderBalance = (await availableBalanceHandler.Handle(balanceQuery, cancellationToken)).Amount;

                    // Return the updated account balance as a result.
                    return await Task.Run(() => new Result(senderBalance));
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
