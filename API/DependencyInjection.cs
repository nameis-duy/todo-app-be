﻿using API.Middlewares;
using Asp.Versioning;
using Microsoft.OpenApi.Models;

namespace API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddControllers();
            services
                .AddRouting(opt => opt.LowercaseUrls = true)
                .AddExceptionHandlerMiddle()
                .AddSecurity()
                .AddConfigFile(builder.Configuration)
                .AddVersioning()
                .AddEndpointsApiExplorer()
                .AddSwagger()
                .AddHttpContextAccessor()
                .AddMemoryCache();

            return services;
        }

        public static IServiceCollection AddConfigFile(this IServiceCollection services, ConfigurationManager configuration)
        {
            configuration.AddJsonFile("appsecret.json", false, true);
            return services;
        }

        public static IServiceCollection AddExceptionHandlerMiddle(this IServiceCollection services)
        {
            services
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails();

            return services;
        }

        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(poli =>
                {
                    poli.WithOrigins("http://localhost:4200");
                    poli.AllowAnyHeader();
                    poli.AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1);
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("X-Version"));
            })
                .AddMvc()
                .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            //Config jwt authen for swagger
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthCore Api", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Jwt Authentication",
                    Description = "Enter you token to authenticate user",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                };

                setup.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                };

                setup.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }
    }
}
