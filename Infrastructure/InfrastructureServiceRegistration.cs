using Application.Contracts;
using Application.Contracts.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBusScheduleRepository, BusScheduleRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IPassengerRepository, PassengerRepository>();

            return services;
        }
    }
}
