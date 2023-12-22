using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailAssyWheelLineRepository
    {
        Task<GetAllEnergyConsumptionAssyWheelLineDto> GetAllEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end);
    }
}
