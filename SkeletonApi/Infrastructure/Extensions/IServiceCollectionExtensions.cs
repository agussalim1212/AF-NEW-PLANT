using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Common.Interfaces;
using SkeletonApi.Infrastructure.Common;
using SkeletonApi.Infrastructure.Options;
using SkeletonApi.Infrastructure.Services;

namespace SkeletonApi.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services)
        {
            services.AddServices();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IMediator, Mediator>()
                .AddTransient<IDomainEventDispatcher, DomainEventDispatcher>()
                .AddTransient<IDateTimeService, DateTimeService>()
                .AddTransient<IEmailService, EmailService>();
        }

        public static void AddHostedMqttClient(this IServiceCollection services, IConfiguration configuration)
        {
            var agentSettings = new AgentMqttClientSettings();
            var brokerSettings = new MqttBrokerSettings();
            configuration.GetSection(nameof(AgentMqttClientSettings)).Bind(agentSettings);
            configuration.GetSection(nameof(MqttBrokerSettings)).Bind(brokerSettings);
            services.AddConfiguredMqttClientService(optionBuilder =>
            {
                optionBuilder
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(agentSettings.Id)
                    .WithCredentials(agentSettings.UserName, agentSettings.Password)
                    .WithTcpServer(brokerSettings.Host, brokerSettings.Port)
                    .Build());
            });

            services.AddTransient<BrokerCommandTopics>();
        }

        private static IServiceCollection AddConfiguredMqttClientService(this IServiceCollection services,
                    Action<AspCoreManagedMqttClientOptionBuilder> configuration)
        {
            services.AddSingleton<ManagedMqttClientOptions>(serviceProvider =>
            {
                var optionsBuilder = new AspCoreManagedMqttClientOptionBuilder(serviceProvider);
                configuration(optionsBuilder);
                return optionsBuilder.Build();
            });
            services.AddSingleton<MqttClientService>();
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                return serviceProvider.GetService<MqttClientService>();
            });
            services.AddTransient<MqttClientServiceProvider>(serviceProvider =>
            {
                var mqttClientService = serviceProvider.GetService<MqttClientService>();
                var mqttClientServiceProvider = new MqttClientServiceProvider(mqttClientService);
                return mqttClientServiceProvider;
            });
            return services;
        }
    }
}