using Microsoft.AspNetCore.SignalR;
using SkeletonApi.IotHub.Commons;
using SkeletonApi.IotHub.Services;

namespace SkeletonApi.IotHub.Hubs
{
    //[Authorize(Policy = PolicyName.AdminOrAgent)]
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