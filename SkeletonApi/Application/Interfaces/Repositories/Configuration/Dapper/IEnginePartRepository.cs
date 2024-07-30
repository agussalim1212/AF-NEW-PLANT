using SkeletonApi.Domain.Entities.Tsdb;

namespace SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper
{
    public interface IEnginePartRepository
    {
        Task Creates(IEnumerable<EnginePart> engineParts);

        Task Creates(IEnumerable<MqttRawValueEntity> mqttrawValues);

        Task Create(IEnumerable<MqttRawValueEntity> listQuality);


        Task<IEnumerable<EnginePart>> FindById(string id);
    }
}