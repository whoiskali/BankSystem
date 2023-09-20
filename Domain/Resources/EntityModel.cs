using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Resources
{
    public class EntityModel : IEntityModel
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; } = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local);
        public DateTime? DeletedDate { get; set; }
    }
    
}
