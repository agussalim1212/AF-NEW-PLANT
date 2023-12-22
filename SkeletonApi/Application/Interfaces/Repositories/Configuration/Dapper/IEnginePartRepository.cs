using SkeletonApi.Domain.Entities.Tsdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories.Dapper
{
    public interface IEnginePartRepository
    {
        Task Creates(IEnumerable<EnginePart> engineParts);

        Task Creates(IEnumerable<MqttRawValueEntity> mqttrawValues);

        Task<IEnumerable<EnginePart>> FindById(string id);
    }
}