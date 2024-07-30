using Microsoft.Extensions.Hosting;

namespace SkeletonApi.Infrastructure.Interfaces
{
    public interface IMqttClientService : IHostedService
    {
        Task PublishAsync(string topic, string payload);
    }
}