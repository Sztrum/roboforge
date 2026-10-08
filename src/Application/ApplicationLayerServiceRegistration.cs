using System.Reflection;
using Microsoft.Extensions.Hosting;
using RoboForge.Application.Common.Behaviours;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationLayerServiceRegistration
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediator((MediatorOptions options) =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors =
            [
                typeof(LogRequestPipelineBehavior<,>),
                typeof(ValidateRequestPipelineBehavior<,>),
                typeof(WarnAboutSlowRequestPipelineBehavior<,>),
            ];
        });
    }
}
