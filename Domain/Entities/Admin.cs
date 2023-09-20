using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string EmployeeNumber { get; set; }

        [ForeignKey(nameof(Id))]
        public User User { get; set; }
    }
}
