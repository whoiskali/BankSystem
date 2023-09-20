using Infrastructure.Persistence.MSSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Application.UnitTest.Abstracts
{
    public abstract class DbContextFactory
    {
        public abstract DbContextOptions<ApplicationDbContext> DbContextOptions { get; }

        public ApplicationDbContext Create()
        {
            var context = new ApplicationDbContext(DbContextOptions);
            return context;
        }
    }
}