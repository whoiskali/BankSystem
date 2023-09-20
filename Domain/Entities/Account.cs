using Domain.Enums;
using Domain.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Domain.Entities
{
    public class Account : EntityModel, IEntityModel
    {
        [Key]
        public int AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public AccountType AccountType { get; set; }
        public string Pin { get; set; }
        public Status Status { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
        [InverseProperty(nameof(Transaction.SenderAccount))]
        public ICollection<Transaction> TransactionsAsSender { get; set; }
        [InverseProperty(nameof(Transaction.ReceiverAccount))]
        public ICollection<Transaction> TransactionsAsReceiver { get; set; }

    }
}
