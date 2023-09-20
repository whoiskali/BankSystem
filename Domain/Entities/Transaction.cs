using Domain.Enums;
using Domain.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Transaction : EntityModel, IEntityModel
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public int? SenderAccountId { get; set; }
        public int? ReceiverAccountId { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey(nameof(SenderAccountId))]
        public Account? SenderAccount { get; set; }
        [ForeignKey(nameof(ReceiverAccountId))]
        public Account? ReceiverAccount { get; set; }
    }
}
