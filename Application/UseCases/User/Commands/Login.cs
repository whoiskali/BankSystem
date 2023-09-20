using Application.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.User.Commands
{
    public class Login
    {
        public record Command(string Email, string Password) : IRequest<Result>;
        public record Result(string Token);
        public record Handler(IApplicationDbContext ApplicationDbContext, ICryptography Cryptography, ITokenClient TokenClient) : IRequestHandler<Command, Result>
        {
            // Handles the login command and generates a token upon successful authentication.
            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                // Find the user in the database by email, ensuring they are not deleted.
                var user = await ApplicationDbContext.Users.SingleOrDefaultAsync(x => x.Email == command.Email && !x.IsDeleted, cancellationToken);

                // If the user is not found, throw an exception indicating incorrect username/password.
                if (user == null)
                    throw new Exception("Username/Password is incorrect.");

                // If the user's status is not active, throw an exception indicating their account status.
                if (user.Status != Status.Active)
                    throw new Exception("Your account is " + user.Status.ToString());

                // Verify the provided password against the stored hashed password.
                if (!Cryptography.Verify(command.Password, user.Password))
                    throw new Exception("Username/Password is incorrect.");

                // Create claims for the authenticated user, including user ID, name, and role.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Type.ToString())
                };

                // Create a claims identity and principal.
                var identity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(identity);

                // Generate a token for the authenticated user.
                var token = TokenClient.Generate(principal);

                // Return the generated token as a result.
                return new Result(token);
            }
        }

    }
}
