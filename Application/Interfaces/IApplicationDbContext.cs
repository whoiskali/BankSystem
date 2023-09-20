using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void UpdateRange([NotNullAttribute] IEnumerable<object> entities);
        EntityEntry Update([NotNullAttribute] object entity);
    }
}
