using GsFashion.Service.Contracts;
using GsFashion.Service.Implementation;
using GsFashion.Service.Services;
using Microsoft.Extensions.DependencyInjection;
namespace GsFashion.Service.Extenstion
{
    public static class ServiceDependancyInjectionconfig
    {
        public static IServiceCollection BuildServiceDependancyInjectionconfig(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IRoleMenuPermissionService,RoleMenuPermissionService>();
            services.AddTransient<IAdminUserService , AdminUserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInventoryItemService, InventoryItemService>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();
            //services.AddTransient<,>();

            return services;
        }
    }
}
