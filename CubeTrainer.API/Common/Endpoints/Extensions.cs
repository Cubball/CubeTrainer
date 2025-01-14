using System.Reflection;

namespace CubeTrainer.API.Common.Endpoints;

internal static class Extensions
{
    public static void AddEndpoints(this IServiceCollection services)
    {
        var @interface = typeof(IEndpoint);
        var endpointTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(@interface));

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(endpointType) as IEndpoint
                ?? throw new InvalidOperationException($"Failed to create an instance of {endpointType.FullName}");
            services.AddSingleton(@interface, endpoint);
        }
    }

    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
        {
            endpoint.Map(app);
        }
    }
}