using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using SkeletonApi.IotHub.Hubs;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SkeletonApi.IotHub.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IManagedMqttClient _managedMqttClient;
        private readonly ManagedMqttClientOptions _options;
        private readonly IHubContext<BrokerHub, IBrokerEvent> _hubContext;
        private readonly IIoTHubEventHandler<MqttRawDataEncapsulation> _mqttStoreEventHandler;
        public MqttClientService(ManagedMqttClientOptions options, IHubContext<BrokerHub, IBrokerEvent> hubContext, IIoTHubEventHandler<MqttRawDataEncapsulation> mqttStoreEventHandler)
        {
            _hubContext = hubContext;
            _options = options;
            _managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            _mqttStoreEventHandler = mqttStoreEventHandler;
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
            var mqttRawDataserialize = new MqttRawDataEncapsulation(
                eventArgs.ApplicationMessage.Topic,
                JsonSerializer.Deserialize<MqttRawData>
               (payload, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(mqttRawDataserialize));
            _mqttStoreEventHandler.Dispatch(mqttRawDataserialize);
            await _hubContext.Clients.All.Broadcast(eventArgs.ApplicationMessage.Topic, payload);
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            await _hubContext.Clients.All.AgentConnectionStatus(true);
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            await _hubContext.Clients.All.AgentConnectionStatus(false);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _managedMqttClient.StartAsync(_options);
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
