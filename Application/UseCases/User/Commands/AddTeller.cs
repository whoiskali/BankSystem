using Application.Interfaces;
using Domain.Entities;
using Mapster;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Account.Commands
{
    public static class AddTeller
    {
        public record Command(CustomerRegistration.Command Information, string EmployeeNumber) : IRequest<Unit>;
        public record Handler(ICryptography Cryptography, IApplicationDbContext ApplicationDbContext) : IRequestHandler<Command, Unit>
        {
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    // Adapt user information from the command to the User entity.
                    var user = command.Information.Adapt<Domain.Entities.User>();

                    // Create a Teller entity for the user.
                    user.Teller = new Domain.Entities.Teller()
                    {
                        EmployeeNumber = command.EmployeeNumber
                    };

                    // Set user status, type, and encrypt the password.
                    user.Status = Domain.Enums.Status.Active;
                    user.Type = Domain.Enums.UserType.Teller;
                    user.Password = Cryptography.Encrypt(user.Password);

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
