using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Persistence.MSSQL
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        readonly ClaimsPrincipal claimsPrincipal;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ClaimsPrincipal claimsPrincipal = null) :
            base(options)
        {
            if (claimsPrincipal != null) this.claimsPrincipal = claimsPrincipal;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
