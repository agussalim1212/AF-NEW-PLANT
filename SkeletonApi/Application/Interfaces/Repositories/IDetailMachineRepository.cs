using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailMachineRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetSubjectPowerAsync(Guid machineId);
        Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetSubjectAirAsync(Guid machineId);
    }
}
