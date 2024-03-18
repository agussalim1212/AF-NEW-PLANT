using MediatR;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AmpereConsumptionDetailMachine;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.VoltageConsumptionDetailMachine
{
    public record GetAllDetailMachineVoltageConsumptionQuery : IRequest<Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Vid { get; set; }
        public string View { get; set; }

        public GetAllDetailMachineVoltageConsumptionQuery(Guid machineId, string type, DateTime startTime, DateTime endTime, string vid, string view)
        {
            MachineId = machineId;
            Type = type;
            Start = startTime;
            End = endTime;
            Vid = vid;
            View = view;
        }

        internal class GetAllDetailMachineVoltageConsumptionQueryHandler : IRequestHandler<GetAllDetailMachineVoltageConsumptionQuery, Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>>
        {
            private readonly IDetailMachineRepository _repository;
            private readonly IDayRepository _dayRepository;
            private readonly IWeekRepository _weekRepository;
            private readonly IMonthRepository _monthRepository;
            private readonly IYearRepository _yearRepository;
            private readonly IDefaultRepository _defaultRepository;
            public GetAllDetailMachineVoltageConsumptionQueryHandler(IDetailMachineRepository repository, IDayRepository dayRepository, IWeekRepository weekRepository, IMonthRepository monthRepository, IYearRepository yearRepository, IDefaultRepository defaultRepository)
            {
                _repository = repository;
                _dayRepository = dayRepository;
                _weekRepository = weekRepository;
                _monthRepository = monthRepository;
                _yearRepository = yearRepository;
                _defaultRepository = defaultRepository;
            }
            public async Task<Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>> Handle(GetAllDetailMachineVoltageConsumptionQuery request, CancellationToken cancellationToken)
            {
                var data = await _repository.GetSubjectAsync(request.MachineId, request.Vid);

                if (request.Type == "day")
                {
                    var dt = await _dayRepository.GetAllDetailMachineCurrentAndVoltageConsumptionDay(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                    return await Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
                }
                else if (request.Type == "week")
                {
                    var dt = await _weekRepository.GetAllDetailMachineCurrentAndVoltageConsumptionWeek(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                    return await Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
                }
                else if (request.Type == "month")
                {
                    var dt = await _monthRepository.GetAllDetailMachineCurrentAndVoltageConsumptionMonth(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                    return await Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
                }
                else if (request.Type == "year")
                {
                    var dt = await _yearRepository.GetAllDetailMachineCurrentAndVoltageConsumptionAsync(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                    return await Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
                }

                var defaultData = await _defaultRepository.GetAllDetailMachineCurrentAndVoltageConsumptionDefault(request.View, data.Vid, data.MachineName, data.SubjectName);
                return await Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>.SuccessAsync(defaultData, "Successfully fetch data");

            }
        }
    }
}
