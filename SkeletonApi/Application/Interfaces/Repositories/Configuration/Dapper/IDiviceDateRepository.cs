using SkeletonApi.Domain.Entities.Tsdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories.Configuration.Dapper
{
    public interface IDiviceDateRepository
    {
       Task Creates(IEnumerable<MqttRawValueEntity> mqttrawValues);
    }
}
