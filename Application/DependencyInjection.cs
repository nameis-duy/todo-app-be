using Application.Interface.Service;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IClaimService, ClaimService>();

            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IEnumService, EnumService>();

            return services;
        }
    }
}
