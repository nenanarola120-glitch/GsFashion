using GsFashion.Repository.Contracts;
using GsFashion.Repository.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace GsFashion.Repository.Extension
{
    public static class RepositoryDependancyInjectionconfig
    {
        public static IServiceCollection BuildRepositoryDependancyInjectionconfig(this IServiceCollection services)
        {
            services.AddTransient<IAdminUserRepository, AdminUserRepo>();
            services.AddTransient<IMenuRepository, MenuRepo>();
            services.AddTransient<IRoleRepository, RoleRepo>();
            services.AddTransient<IRoleMenuPermissionRepository, RoleMenuPermissionRepo>();
            services.AddScoped<ICategoryRepository, CategoryRepo>();
            services.AddScoped<IInventoryItemRepository, InventoryItemRepo>();
            services.AddTransient<IRentalRepository,RentalRepo>();
            services.AddTransient<IRentalPaymentRepository, RentalPaymentRepo>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();


            return services;
        }
    }
}
