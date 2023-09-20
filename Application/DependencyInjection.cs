using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }
    }
}
