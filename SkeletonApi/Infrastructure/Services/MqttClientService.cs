using Microsoft.AspNetCore.SignalR;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkeletonApi.Infrastructure.Interfaces;
using SkeletonApi.Infrastructure.Hubs;

namespace SkeletonApi.Infrastructure.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IManagedMqttClient _managedMqttClient;
        private readonly ManagedMqttClientOptions options;
        private readonly ILogger<MqttClientService> _logger;

        public MqttClientService(ManagedMqttClientOptions options,
            IServiceScopeFactory service, ILogger<MqttClientService> logger)
        {
            //this.hubContext = hubContext;
            this.options = options;
            _managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            _managedMqttClient.ConnectedAsync += HandleConnectedAsync;
            _managedMqttClient.DisconnectedAsync += HandleDisconnectedAsync;
            _managedMqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var payload = System.Text.Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            Console.WriteLine(JsonSerializer.Serialize(payload));
            //await hubContext.Clients.All.Broadcast(eventArgs.ApplicationMessage.Topic, payload);
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            //await hubContext.Clients.All.AgentConnectionStatus(true);
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            //await hubContext.Clients.All.AgentConnectionStatus(false);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _managedMqttClient.StartAsync(options);
            await _managedMqttClient.SubscribeAsync("#");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _managedMqttClient.StopAsync();
        }

        public async Task PublishAsync(string topic, string payload)
        {
            var applicationMessage = new MqttApplicationMessageBuilder().WithTopic(topic)
                                                                        .WithPayload(payload)
                                                                        .Build();
            var manegedApplicationMessage = new ManagedMqttApplicationMessageBuilder().WithApplicationMessage(applicationMessage)
                                                                                      .Build();
            await _managedMqttClient.EnqueueAsync(manegedApplicationMessage);
        }
    }
}
