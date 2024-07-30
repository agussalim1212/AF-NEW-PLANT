using SkeletonApi.Infrastructure.Interfaces;

namespace SkeletonApi.Infrastructure.Services
{
    public class MqttClientServiceProvider
    {
        public IMqttClientService MqttClientService { get; set; }

        public MqttClientServiceProvider(IMqttClientService mqttClientService)
        {
            MqttClientService = mqttClientService;
        }
    }
}