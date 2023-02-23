using Microsoft.Extensions.DependencyInjection;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Interfaces;
using WebApplication.Infrastructure.Services;

namespace WebApplication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddDbContext<InMemoryContext>();

            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
