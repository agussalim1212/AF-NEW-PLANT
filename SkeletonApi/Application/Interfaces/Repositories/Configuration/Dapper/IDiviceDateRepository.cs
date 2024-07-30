using SkeletonApi.Domain.Entities.Tsdb;

namespace SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper
{
    public interface IDiviceDateRepository
    {
        Task Creates(IEnumerable<MqttRawValueEntity> mqttrawValues);
     

    }
}