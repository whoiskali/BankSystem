using Application.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Customer.Commands
{
    public static class AddCustomer
    {
        public record Command(string FirstName, string LastName, string Address, string Phone, string Email) : IRequest<Result>;
        public record Result(Guid Customer, string FirstName, string LastName, string Address, string Phone, string Email, string Password);
        public record Handler(ICryptography Cryptography, IApplicationDbContext ApplicationDbContext) : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    // Convert the command to a user entity.
                    var user = command.Adapt<Domain.Entities.User>();

                    // Generate a password based on the user's name.
                    var password = user.LastName + user.FirstName;

                    // Encrypt the password.
                    user.Password = Cryptography.Encrypt(password);

                    // Set user properties.
                    user.Status = Domain.Enums.Status.Active;
                    user.Type = Domain.Enums.UserType.Customer;
                    user.Customer = new Domain.Entities.Customer();

                    // Add the user to the database.
                    var data = ApplicationDbContext.Users.Add(user);

                    // Save changes to the database.
                    await ApplicationDbContext.SaveChangesAsync(cancellationToken);

                    // Return the result of the registration operation.
                    return await Task.Run(() => new Result(data.Entity.Id, data.Entity.FirstName, data.Entity.LastName, data.Entity.Address, data.Entity.Phone, data.Entity.Email, password));
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
