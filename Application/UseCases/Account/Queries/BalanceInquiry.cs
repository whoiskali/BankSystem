using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Account.Queries
{
    public static class BalanceInquiry
    {
        public record Query(int AccountNumber) : IRequest<Result>;
        public record Result(decimal Amount);
        public record Handler(IApplicationDbContext ApplicationDbContext) : IRequestHandler<Query, Result>
        {
            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                try
                {
                    // Retrieve the account based on the account number.
                    var account = await ApplicationDbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == query.AccountNumber);

                    // Check if the account exists.
                    if (account == null) throw new Exception("Account doesn't exist.");

                    // Calculate the total received amount for the account.
                    var received = await ApplicationDbContext.Transactions.Where(x => x.ReceiverAccount == account).SumAsync(x => x.Amount);

                    // Calculate the total sent amount for the account.
                    var sent = await ApplicationDbContext.Transactions.Where(x => x.SenderAccount == account).SumAsync(x => x.Amount);

                    // Calculate the balance (received - sent) and return the result.
                    return new Result(received - sent);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
