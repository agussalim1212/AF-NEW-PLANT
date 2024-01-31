using MediatR;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine
{
  
    public record GetAllDetailMachineAirAndElectricConsumptionQuery : IRequest<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string View { get; set; }
        public GetAllDetailMachineAirAndElectricConsumptionQuery(string view, Guid machineId, string type, DateTime startTime, DateTime endTime)
        {
            MachineId = machineId;
            Type = type;
            Start = startTime;
            End = endTime;
            View = view;
        }
    }

    internal class GetAllDetailMachineAirConsumptionQueryHandler : IRequestHandler<GetAllDetailMachineAirAndElectricConsumptionQuery, Result<GetAllDetailMachineAirAndElectricConsumptionDto>>
    {
        private readonly IDetailMachineRepository _repository;
        private readonly IDayRepository _dayRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IMonthRepository _monthRepository;
        private readonly IYearRepository _yearRepository;
        private readonly IDefaultRepository _defaultRepository;
        public GetAllDetailMachineAirConsumptionQueryHandler(IDetailMachineRepository repository, IDayRepository dayRepository, IWeekRepository weekRepository, IMonthRepository monthRepository, IYearRepository yearRepository, IDefaultRepository defaultRepository)
        {
            _repository = repository;
            _dayRepository = dayRepository;
            _weekRepository = weekRepository;
            _monthRepository = monthRepository;
            _yearRepository = yearRepository;
            _defaultRepository = defaultRepository;
        }
        public async Task<Result<GetAllDetailMachineAirAndElectricConsumptionDto>> Handle(GetAllDetailMachineAirAndElectricConsumptionQuery request, CancellationToken cancellationToken)
        {
            var data = await _repository.GetSubjectAirAsync(request.MachineId);
            string view = "air_consumption_setting";
            if (request.Type == "day")
            {
                var dt = await _dayRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(view, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "week")
            {
                var dt = await _weekRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "month")
            {
                var dt = await _monthRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "year")
            {
                var dt = await _yearRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }

            var defaultData = await _defaultRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(data.Vid, data.MachineName, data.SubjectName);
            return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(defaultData,"Successfully fetch data");

        }
    }
}
