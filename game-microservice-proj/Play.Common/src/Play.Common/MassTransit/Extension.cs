using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extension
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            //MassTransist
            services.AddMassTransit(configure =>
            {
                /*Assembly.GetEntryAssembly(): This retrieves the assembly that contains the entry point of the application. In an ASP.NET Core application, it typically refers to the assembly where your application starts, often the main project assembly.

                configure.AddConsumers(...): This method, when provided with an assembly, scans that assembly to find classes that implement the MassTransit IConsumer<T> interface. These classes are responsible for consuming and processing incoming messages of specific types T.*/
                configure.AddConsumers(Assembly.GetEntryAssembly());
                configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configurator.Host(rabbitMQSettings.Host);
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    configurator.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });
            //Starts the rabbitmq bus so that messages can be published
            // builder.Services.AddMassTransitHostedService(); no longer req in latest version
            return services;
        }
    }
}
