using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using FluentValidation;
using Infrastructure.Implement;
using Infrastructure.Implement.Repository;
using Infrastructure.Implement.Service;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthen(config)
                .AddPersistance(config)
                .AddServices()
                .AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITimeService, TimeService>();
            services.AddScoped<IClaimService, ClaimService>();

            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }

        public static IServiceCollection AddAuthen(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSetting>(config.GetSection(JwtSetting.Section));

            services
                .ConfigureOptions<TokenValidationConfiguration>()
                .AddAuthentication(scheme =>
                {
                    scheme.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    scheme.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DbConnectionString") ?? throw new KeyNotFoundException("DbConnectionString not found")));

            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<IAccountRepo, AccountRepo>();
            services.AddScoped<ITaskRepo, TaskRepo>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
