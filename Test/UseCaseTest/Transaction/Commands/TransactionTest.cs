using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Application.UseCases.Account.Queries;
using Application.UseCases.Transaction.Commands;
using DeliveryService.Application.UnitTest.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace Applicaton.UnitTest.UseCaseTest.Commands
{
    using Deposit = Application.UseCases.Transaction.Commands.Deposit;
    using WithdrawCash = Application.UseCases.Transaction.Commands.WithdrawCash;
    using TransferMoney = Application.UseCases.Transaction.Commands.TransferMoney;

    /// <summary>
    /// This class contains unit tests for the transaction-related use cases.
    /// </summary>
    public class TransactionTest : IDisposable
    {
        readonly User test;
        readonly ClaimsPrincipal login;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionTest"/> class and sets up the test data.
        /// </summary>
        public TransactionTest()
        {
            using var db = new InMemoryDbContext().Create();
            test = new User
            {
                Address = "",
                Email = "",
                FirstName = "",
                LastName = "",
                Phone = "",
                Password = "",
                Customer = new Customer
                {
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            Pin = "",
                            AccountType = 0,
                            Status = Domain.Enums.Status.Active,
                            TransactionsAsReceiver = new List<Transaction>
                            {
                                new Transaction
                                {
                                    Amount = 1000,
                                    Type = Domain.Enums.TransactionType.Deposit,
                                }
                            }
                        },
                        new Account
                        {
                            Pin = "",
                            AccountType = 0,
                            Status = Domain.Enums.Status.Disabled,
                            TransactionsAsReceiver = new List<Transaction>
                            {
                                new Transaction
                                {
                                    Amount = 2000,
                                    Type = Domain.Enums.TransactionType.Deposit,
                                }
                            }
                        },
                        new Account
                        {
                            Pin = "",
                            AccountType = 0,
                            Status = Domain.Enums.Status.Active,
                            TransactionsAsReceiver = new List<Transaction>
                            {
                                new Transaction
                                {
                                    Amount = 2000,
                                    Type = Domain.Enums.TransactionType.Deposit,
                                }
                            }
                        }
                    }
                }
            };

            db.Users.Add(test);
            db.SaveChanges();

            login = new ClaimsPrincipal(
                new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier,test.Id.ToString()),
                    new Claim(ClaimTypes.Name, test.Email),
                    new Claim(ClaimTypes.Role, test.Type.ToString())
                })
            );
        }

        /// <summary>
        /// Tests the deposit operation.
        /// </summary>
        [Fact]
        public async void Should_Deposit()
        {
            using var db = new InMemoryDbContext().Create();
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var commandBalance = new BalanceInquiry.Query(testUser.Customer.Accounts.FirstOrDefault().AccountNumber);

            var resultAvailable = await new BalanceInquiry.Handler(db).Handle(commandBalance, new CancellationToken());

            var command = new Deposit.Command(testUser.Customer.Accounts.FirstOrDefault().AccountNumber, 2000);
            var handler = await new Deposit.Handler(db).Handle(command, new CancellationToken());

            var result = await new BalanceInquiry.Handler(db).Handle(commandBalance, new CancellationToken());
            Assert.Equal(resultAvailable.Amount + 2000, result.Amount);
        }

        /// <summary>
        /// Tests the deposit operation when the account is not found.
        /// </summary>
        [Fact]
        public async void Should_Not_Deposit()
        {
            using var db = new InMemoryDbContext().Create();
            var command = new Deposit.Command(1234, 2000);
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await new Deposit.Handler(db).Handle(command, new CancellationToken());
            });
            Assert.Equal("Account not found.", exception.Message);
        }

        /// <summary>
        /// Tests the withdrawal operation.
        /// </summary>
        [Fact]
        public async void Should_Withdraw()
        {
            using var db = new InMemoryDbContext().Create();
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var command = new WithdrawCash.Command(testUser.Customer.Accounts.First().AccountNumber, 1000);
            var handler = await new WithdrawCash.Handler(db).Handle(command, new CancellationToken());

            var commandBalance = new BalanceInquiry.Query(testUser.Customer.Accounts.First().AccountNumber);

            var result = await new BalanceInquiry.Handler(db).Handle(commandBalance, new CancellationToken());
            Assert.Equal(0, result.Amount);
        }

        /// <summary>
        /// Tests the withdrawal operation when no account is found.
        /// </summary>
        [Fact]
        public async void Should_Not_Withdraw_No_Account_Found()
        {
            using var db = new InMemoryDbContext().Create();
            var command = new WithdrawCash.Command(1234, 1000);
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await new WithdrawCash.Handler(db).Handle(command, new CancellationToken());
            });
            Assert.Equal("Account not found.", exception.Message);
        }

        /// <summary>
        /// Tests the withdrawal operation with insufficient balance.
        /// </summary>
        [Fact]
        public async void Should_Not_Withdraw_Insufficient_Balance()
        {
            using var db = new InMemoryDbContext().Create();
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var command = new WithdrawCash.Command(testUser.Customer.Accounts.FirstOrDefault().AccountNumber, 1000);
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await new WithdrawCash.Handler(db).Handle(command, new CancellationToken());
            });
            Assert.Equal("Insufficient balance.", exception.Message);
        }

        /// <summary>
        /// Tests the money transfer operation.
        /// </summary>
        [Fact]
        public async void Should_TransferMoney()
        {
            using var db = new InMemoryDbContext().Create();
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var commandBalance = new BalanceInquiry.Query(testUser.Customer.Accounts.FirstOrDefault().AccountNumber);
            var resultAvailable = await new BalanceInquiry.Handler(db).Handle(commandBalance, new CancellationToken());

            var command = new TransferMoney.Command(testUser.Customer.Accounts.FirstOrDefault().AccountNumber, testUser.Customer.Accounts.Last().AccountNumber, 1000);
            var handler = await new TransferMoney.Handler(db).Handle(command, new CancellationToken());

            var result = await new BalanceInquiry.Handler(db).Handle(commandBalance, new CancellationToken());
            Assert.Equal(resultAvailable.Amount - 1000, result.Amount);
        }

        /// <summary>
        /// Tests the money transfer operation when the sender account is not found.
        /// </summary>
        [Fact]
        public async void Should_Not_TransferMoney_No_Sender_Account_Found()
        {
            using var db = new InMemoryDbContext().Create();
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var command = new TransferMoney.Command(1234, testUser.Customer.Accounts.LastOrDefault().AccountNumber, 1000);
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await new TransferMoney.Handler(db).Handle(command, new CancellationToken());
            });
            Assert.Equal("Sender account not found.", exception.Message);
        }

        /// <summary>
        /// Tests the money transfer operation when the receiver account is not found.
        /// </summary>
        [Fact]
        public async void Should_Not_TransferMoney_No_Receiver_Account_Found()
        {
            using var db = new InMemoryDbContext().Create();
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var command = new TransferMoney.Command(testUser.Customer.Accounts.FirstOrDefault().AccountNumber, 1234, 1000);
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await new TransferMoney.Handler(db).Handle(command, new CancellationToken());
            });
            Assert.Equal("Receiver account not found.", exception.Message);
        }

        /// <summary>
        /// Tests the money transfer operation when the account is not active.
        /// </summary>
        [Fact]
        public async void Should_Not_TransferMoney_Account_Is_Not_Active()
        {
            using var db = new InMemoryDbContext().Create();
            var accountNotActive = db.Accounts.FirstOrDefault(x => x.Status != Domain.Enums.Status.Active);
            var testUser = db.Users.Include(x => x.Customer).ThenInclude(x => x.Accounts).FirstOrDefault();
            var command = new TransferMoney.Command(testUser.Customer.Accounts.FirstOrDefault().AccountNumber, accountNotActive.AccountNumber, 1000);
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await new TransferMoney.Handler(db).Handle(command, new CancellationToken());
            });
            Assert.Equal("Account is not active.", exception.Message);
        }

        /// <summary>
        /// Cleans up resources and removes test data.
        /// </summary>
        public void Dispose()
        {
            using var db = new InMemoryDbContext().Create();
            var entity = db.Transactions.Find(test.Id);
            if (entity != null)
            {
                db.Transactions.Remove(entity);
                db.SaveChangesAsync();
            }
        }
    }
}

