
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SkeletonApi.Infrastructure.Common;
using SkeletonApi.Infrastructure.Interfaces;
using SkeletonApi.Infrastructure.Services;

namespace SkeletonApi.Infrastructure.Hubs
{
    public class BrokerHub : Hub<IBrokerEvent>
    {
        private readonly IMqttClientService mqttClientService;
        private readonly BrokerCommandTopics CommandTopics;
        public BrokerHub(MqttClientServiceProvider provider, BrokerCommandTopics commandTopics)
        {
            mqttClientService = provider.MqttClientService;
            CommandTopics = commandTopics;
        }

        public async Task RequestMqttBroker(string topic, string payload)
        {
            await mqttClientService.PublishAsync(topic, payload);
        }
    }
}