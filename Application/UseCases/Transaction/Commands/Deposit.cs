using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Transaction.Commands
{
    public static class Deposit
    {
        public record Command(int AccountNumber, decimal Amount) : IRequest<Unit>;
        public record Handler(IApplicationDbContext ApplicationDbContext) : IRequestHandler<Command, Unit>
        {
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                { 
                    // Retrieve the account based on the account number.
                    var account = await ApplicationDbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == command.AccountNumber);

                    // Check if the account exists.
                    if (account == null) throw new Exception("Account not found.");

                    // Create a new transaction for deposit.
                    var transaction = new Domain.Entities.Transaction()
                    {
                        Type = TransactionType.Deposit,
                        Amount = command.Amount,
                        ReceiverAccount = account,
                        SenderAccount = null
                    };

                    // Add the transaction to the database.
                    var value = ApplicationDbContext.Transactions.Add(transaction);

                    // Save changes to the database.
                    await ApplicationDbContext.SaveChangesAsync(cancellationToken);

                    // Return a successful result.
                    return await Task.Run(() => Unit.Value);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
