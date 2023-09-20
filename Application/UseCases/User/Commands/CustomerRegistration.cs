using Application.Interfaces;
using Domain.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Account.Commands
{
    public class CustomerRegistration
    {
        public record Command(string FirstName, string LastName, string Address, string Phone, string Email, string Password) : IRequest<Unit>;
        public record Handler(ICryptography Cryptography, IApplicationDbContext ApplicationDbContext) : IRequestHandler<Command, Unit>
        {
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    // Adapt user information from the command to the User entity.
                    var user = command.Adapt<Domain.Entities.User>();

                    // Encrypt the user's password.
                    user.Password = Cryptography.Encrypt(user.Password);

                    // Set user status, type, and create a Customer entity.
                    user.Status = Domain.Enums.Status.Active;
                    user.Type = Domain.Enums.UserType.Customer;
                    user.Customer = new Domain.Entities.Customer();

                    // Add the user to the database.
                    ApplicationDbContext.Users.Add(user);

                    // Save changes to the database.
                    await ApplicationDbContext.SaveChangesAsync(cancellationToken);

                    // Return a successful result (Unit).
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
