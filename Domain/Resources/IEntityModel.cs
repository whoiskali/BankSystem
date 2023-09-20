using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Resources
{
    public interface IEntityModel
    {
        DateTime CreatedDate { get; }
    }
}
