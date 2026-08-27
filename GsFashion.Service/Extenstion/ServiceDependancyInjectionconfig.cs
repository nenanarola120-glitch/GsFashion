using GsFashion.Service.Contracts;
using GsFashion.Service.Services;
using Microsoft.Extensions.DependencyInjection;
namespace GsFashion.Service.Extenstion
{
    public static class ServiceDependancyInjectionconfig
    {
        public static IServiceCollection BuildServiceDependancyInjectionconfig(this IServiceCollection services)
        {
            services.AddScoped<IExtraService, ExtraService>();

            return services;
        }
    }
}
