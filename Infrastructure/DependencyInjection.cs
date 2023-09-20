using Application.Interfaces;
using Infrastructure.Models.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Cryptographies;
using Infrastructure.Persistence.MSSQL;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, string connectionString, string assembly, Jwt jwt)
        {
            services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                        connectionString,
                        b => b.MigrationsAssembly(assembly)
                    )
                );
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<ICryptography, Cryptography>();
            services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = jwt.ValidateIssuer ?? true,
                            ValidateAudience = jwt.ValidateAudience ?? true,
                            ValidAudience = jwt.ValidAudience,
                            ValidIssuer = jwt.ValidIssuer,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret))
                        };
                    });
            
            services.AddScoped<ITokenClient>(x => ActivatorUtilities.CreateInstance<JwtToken>(x, jwt));
        }
    }
}
