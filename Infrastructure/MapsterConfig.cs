using Application.DTOs.Task;
using Domain.Entity;
using Domain.Enum.Task;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class MapsterConfig
    {
        public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            TypeAdapterConfig<Tasks, TaskVM>
                .NewConfig()
                .Map(dest => dest.IsCompleted, src => src.Status == Status.Completed)
                .Map(dest => dest.Priority, src => Enum.GetName(typeof(Priority), src.Priority))
                .Map(dest => dest.Status, src => Enum.GetName(typeof(Status), src.Status));

            return services;
        }
    }
}
