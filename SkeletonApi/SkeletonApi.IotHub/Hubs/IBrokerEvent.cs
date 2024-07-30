namespace SkeletonApi.IotHub.Hubs
{
    public interface IBrokerEvent
    {
        Task Broadcast(string topic, string payload);

        Task AgentConnectionStatus(bool isConnected);
    }
}