using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICryptography
    {
        string Encrypt(string Password);
        bool Verify(string Password, string Hashed);
    }
}
