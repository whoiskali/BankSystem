using Application.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Account.Commands
{
    public static class OpenAccount
    {
        public record Command(Guid CustomerId, AccountType AccountType) : IRequest<Result>;
        public record Result(string AccountName, int AccountNumber, string AccountType, int Pin);
        public record Handler(IApplicationDbContext ApplicationDbContext, ICryptography Cryptography) : IRequestHandler<Command, Result>
        {
            // Handles the opening of a new account.
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    // Retrieve the customer information.
                    var customer = await ApplicationDbContext.Customers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == command.CustomerId);

                    // Check if the customer exists.
                    if (customer == null) throw new Exception("No customer found.");

                    // Generate a random PIN and account number.
                    var random = new Random();
                    var pin = random.Next(100000, 999999);

                    // Create a new account for the customer.
                    var account = new Domain.Entities.Account()
                    {
                        AccountType = command.AccountType,
                        Pin = Cryptography.Encrypt(pin.ToString()),
                        Customer = customer,
                        Status = Status.Active
                    };

                    // Add the account to the database.
                    var value = ApplicationDbContext.Accounts.Add(account);

                    // Save changes to the database.
                    await ApplicationDbContext.SaveChangesAsync(cancellationToken);

                    // Return the result of the account opening operation.
                    return await Task.Run(() => new Result(customer.User.LastName + ", " + customer.User.FirstName, value.Entity.AccountNumber, account.AccountType.ToString(), pin));
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
