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
    public static class TransferMoney
    {
        public record Command(int SenderAccountNumber, int ReceiverAccountNumber, decimal Amount) : IRequest<Result>;
        public record Result(decimal AvailableBalance);
        public record Handler(IApplicationDbContext ApplicationDbContext) : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    var senderAccount = await ApplicationDbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == command.SenderAccountNumber);

                    // Check if the sender account exists.
                    if (senderAccount == null) throw new Exception("Sender account not found.");

                    var receiverAccount = await ApplicationDbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == command.ReceiverAccountNumber);

                    // Check if the receiver account exists.
                    if (receiverAccount == null) throw new Exception("Receiver account not found.");

                    // Check if the sender account is active.
                    if (senderAccount.Status != Status.Active) throw new Exception("Account is not active.");

                    // Query the available balance of the sender account.
                    var balanceQuery = new BalanceInquiry.Query(senderAccount.AccountNumber);
                    var availableBalanceHandler = new BalanceInquiry.Handler(ApplicationDbContext);
                    var senderAvailableBalance = (await availableBalanceHandler.Handle(balanceQuery, cancellationToken)).Amount;

                    // Check if there is sufficient balance for the transfer.
                    if ((senderAvailableBalance - command.Amount) < 0)
                        throw new Exception("Insufficient balance");

                    // Create a new transaction for the transfer.
                    var transaction = new Domain.Entities.Transaction()
                    {
                        Type = TransactionType.Transfer,
                        Amount = command.Amount,
                        ReceiverAccount = receiverAccount,
                        SenderAccount = senderAccount
                    };

                    // Add the transaction to the database.
                    var value = ApplicationDbContext.Transactions.Add(transaction);

                    // Save changes to the database.
                    await ApplicationDbContext.SaveChangesAsync(cancellationToken);

                    // Query the sender's updated balance.
                    var senderBalance = (await availableBalanceHandler.Handle(balanceQuery, cancellationToken)).Amount;

                    // Return the sender's updated balance as a result.
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
