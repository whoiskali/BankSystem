﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(Id))]
        public User User { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
