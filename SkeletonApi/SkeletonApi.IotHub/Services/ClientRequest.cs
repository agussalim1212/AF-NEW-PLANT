namespace SkeletonApi.IotHub.Services
{
    public class ClientRequest : IClientRequest
    {
        private readonly IMqttClientService _mqttClientService;

        public ClientRequest(MqttClientServiceProvider mqttClientService)
        {
            _mqttClientService = mqttClientService.MqttClientService;
        }

        public async Task GetTask(string topic, string messages)
        {
            await _mqttClientService.PublishAsync(topic, messages);
            Task.CompletedTask.Wait();
        }
    }
}