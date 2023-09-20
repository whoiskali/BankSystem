using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cryptographies
{
    public class Cryptography: ICryptography
    {
        public string Encrypt(string Password)
        {
            string password = BCrypt.Net.BCrypt.HashPassword(Password);
            return password;
        }
        public bool Verify(string Password, string Hashed)
        {
            bool isVerified = BCrypt.Net.BCrypt.Verify(Password, Hashed);
            return isVerified;
        }
    }
}
