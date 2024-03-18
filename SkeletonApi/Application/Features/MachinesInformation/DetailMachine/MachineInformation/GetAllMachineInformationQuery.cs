using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation
{
    public record GetAllMachineInformationQuery : IRequest<Result<GetAllMachineInformationDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetAllMachineInformationHandler : IRequestHandler<GetAllMachineInformationQuery, Result<GetAllMachineInformationDto>>
    {
        private readonly IDetailMachineRepository _detailMachineRepository;

        public GetAllMachineInformationHandler(IDetailMachineRepository detailMachineRepository)
        {
            _detailMachineRepository = detailMachineRepository;
        }

        public async Task<Result<GetAllMachineInformationDto>> Handle(GetAllMachineInformationQuery query, CancellationToken cancellationToken)
        {

            var vidRunning = await _detailMachineRepository.GetSubjectAsync(query.MachineId, "RUNNING-HOUR");
            var vidReminder = await _detailMachineRepository.GetSubjectAsync(query.MachineId, "REMINDER-CALIBRATION");

            var data = await _detailMachineRepository.GetAllMachineInformationAsync(query.MachineId, vidRunning.Vid, vidReminder.Vid, vidRunning.MachineName);
            return await Result<GetAllMachineInformationDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
