namespace SkeletonApi.IotHub.Services
{
    public interface IMqttClientService : IHostedService
    //IMqttClientConnectedHandler,
    //IMqttClientDisconnectedHandler,
    //IMqttApplicationMessageReceivedHandler
    {
        Task PublishAsync(string topic, string payload);
    }
}