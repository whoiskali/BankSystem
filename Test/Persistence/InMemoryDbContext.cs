using DeliveryService.Application.UnitTest.Abstracts;
using Infrastructure.Persistence.MSSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace DeliveryService.Application.UnitTest.Persistence
{
    public class InMemoryDbContext : DbContextFactory
    {
        public override DbContextOptions<ApplicationDbContext> DbContextOptions =>
            new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase(new Guid().ToString())
                        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                        .Options;
    }
}