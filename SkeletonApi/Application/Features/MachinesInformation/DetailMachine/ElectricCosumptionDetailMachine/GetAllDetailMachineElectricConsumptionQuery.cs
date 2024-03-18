using MediatR;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.ElectricCosumptionDetailMachine
{
    public record GetAllDetailMachineElectricConsumptionQuery : IRequest<Result<GetAllDetailMachineAirConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string View { get; set; }
        public string Vid { get; set; }
        public GetAllDetailMachineElectricConsumptionQuery(Guid machineId, string type, DateTime startTime, DateTime endTime, string view, string vid)
        {
            MachineId = machineId;
            Type = type;
            Start = startTime;
            End = endTime;
            View = view;
            Vid = vid;
        }
    }

    internal class GetAllDetailMachineElectricConsumptionQueryHandler : IRequestHandler<GetAllDetailMachineElectricConsumptionQuery, Result<GetAllDetailMachineAirConsumptionDto>>
    {
        private readonly IDetailMachineRepository _repository;
        private readonly IDayRepository _dayRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IMonthRepository _monthRepository;
        private readonly IYearRepository _yearRepository;
        private readonly IDefaultRepository _defaultRepository;
        public GetAllDetailMachineElectricConsumptionQueryHandler(IDetailMachineRepository repository, IDayRepository dayRepository, IWeekRepository weekRepository, IMonthRepository monthRepository, IYearRepository yearRepository, IDefaultRepository defaultRepository)
        {
            _repository = repository;
            _dayRepository = dayRepository;
            _weekRepository = weekRepository;
            _monthRepository = monthRepository;
            _yearRepository = yearRepository;
            _defaultRepository = defaultRepository;
        }
        public async Task<Result<GetAllDetailMachineAirConsumptionDto>> Handle(GetAllDetailMachineElectricConsumptionQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.GetSubjectAsync(request.MachineId, request.Vid);

            if (request.Type == "day")
            {
                var dt = await _dayRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "week")
            {
                var dt = await _weekRepository.GetAllDetailMachineAirAndElectricConsumptionWeek(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "month")
            {
                var dt = await _monthRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "year")
            {
                var dt = await _yearRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
               return await Result<GetAllDetailMachineAirConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }

            var defaultData = await _defaultRepository.GetAllDetailMachineAirAndElectricConsumptionDefault(request.View, data.Vid, data.MachineName, data.SubjectName);
            return await Result<GetAllDetailMachineAirConsumptionDto>.SuccessAsync(defaultData, "Successfully fetch data");

        }
    }
}
