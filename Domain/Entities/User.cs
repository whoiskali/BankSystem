using Domain.Enums;
using Domain.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : EntityModel, IEntityModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Status Status { get; set; }
        public UserType Type { get; set; }

        public Customer? Customer { get; set; }
        public Admin? Admin { get; set; }
        public Teller? Teller { get; set; }
    }
}
