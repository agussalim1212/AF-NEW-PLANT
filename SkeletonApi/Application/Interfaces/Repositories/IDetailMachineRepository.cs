using SkeletonApi.Application.DTOs.DetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation;
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
        Task<GetVidSubjectDto> GetSubjectAsync(Guid machineId, string vid);
        Task<GetAllMachineInformationDto> GetAllMachineInformationAsync(Guid machine_id, string vidRunning, string vidReminder, string machineName);

    }
}
