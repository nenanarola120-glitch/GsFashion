using GsFashion.Repository.Contracts;
using GsFashion.Repository.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace GsFashion.Repository.Extension
{
    public static class RepositoryDependancyInjectionconfig
    {
        public static IServiceCollection BuildRepositoryDependancyInjectionconfig(this IServiceCollection services)
        {
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            //services.AddScoped<IAuthRepository, AuthRepository>();
            //services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IExtraRepo, ExtraRepo>();

            return services;
        }
    }
}
