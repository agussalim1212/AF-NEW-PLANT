namespace SkeletonApi.IotHub.Services
{
    public interface IClientRequest
    {
        Task GetTask(string topic, string messages);
    }
}
