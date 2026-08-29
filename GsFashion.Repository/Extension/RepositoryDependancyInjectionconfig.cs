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
            services.AddTransient<IAdminUserRepository, AdminUserRepo>();
            services.AddTransient<IMenuRepository, MenuRepo>();
            services.AddTransient<IRoleRepository, RoleRepo>();
 

            return services;
        }
    }
}
