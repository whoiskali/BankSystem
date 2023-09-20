using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Infrastructure.Models.Token;
using Application.Interfaces;

namespace Infrastructure.Token
{
    public class JwtToken : ITokenClient
    {
        readonly Jwt jwt;
        public JwtToken(Jwt jwt)
        {
            this.jwt = jwt;
        }
        public string Generate(ClaimsPrincipal claimsPrincipal)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));

            var token = new JwtSecurityToken(
                issuer: jwt.ValidIssuer,
                audience: jwt.ValidAudience,
                expires: DateTime.Now.AddMonths(1),
                claims: claimsPrincipal.Claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string Validate()
        {
            throw new NotImplementedException();
        }
    }
}
