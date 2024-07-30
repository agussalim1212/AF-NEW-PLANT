using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine
{
    public record GetAllDetailMachineEnergyConsumptionQuery : IRequest<Result<GetAllDetailMachineEnergyConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public GetAllDetailMachineEnergyConsumptionQuery(Guid machineId, string type, DateTime? startTime, DateTime? endTime)
        {
            MachineId = machineId;
            Type = type;
            Start = startTime;
            End = endTime;
        }
    }

    internal class GetAllDetailMachineEnergyConsumptionQueryHandler : IRequestHandler<GetAllDetailMachineEnergyConsumptionQuery, Result<GetAllDetailMachineEnergyConsumptionDto>>
    {
        private readonly IDetailMachineRepository _repository;
        private readonly IDayRepository _dayRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IMonthRepository _monthRepository;
        private readonly IYearRepository _yearRepository;
        private readonly IDefaultRepository _defaultRepository;

        public GetAllDetailMachineEnergyConsumptionQueryHandler(IDetailMachineRepository repository, IDayRepository dayRepository, IWeekRepository weekRepository, IMonthRepository monthRepository, IYearRepository yearRepository, IDefaultRepository defaultRepository)
        {
            _repository = repository;
            _dayRepository = dayRepository;
            _weekRepository = weekRepository;
            _monthRepository = monthRepository;
            _yearRepository = yearRepository;
            _defaultRepository = defaultRepository;
        }

        public async Task<Result<GetAllDetailMachineEnergyConsumptionDto>> Handle(GetAllDetailMachineEnergyConsumptionQuery request, CancellationToken cancellationToken)
        {
            //untuk mendapatkan nama subject berdasarkan id machine yg dikirim dan vid
            var data = await _repository.GetSubjectPowerAsync(request.MachineId);

            //jika type == day maka data akan di arahkan ke proses filtering day yang berada di SkeletonApi.Application.Interfaces.Repositories.Filtering, didalam repositories tsb 
            //terdapat method untuk filtering tergantung type yang di pilih
            if (request.Type == "day")
            {
                var dt = await _dayRepository.GetAllDetailMachineEnergyConsumptionAsync(data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineEnergyConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "week")
            {
                var dt = await _weekRepository.GetAllDetailMachineEnergyConsumptionWeek(data.Vid, data.MachineName, data.SubjectName, request.Start.Value, request.End.Value);
                return await Result<GetAllDetailMachineEnergyConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "month")
            {
                var dt = await _monthRepository.GetAllDetailMachineEnergyConsumptionMonth(data.Vid, data.MachineName, data.SubjectName, request.Start.Value, request.End.Value);
                return await Result<GetAllDetailMachineEnergyConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "year")
            {
                var dt = await _yearRepository.GetAllDetailMachineEnergyConsumptionYear(data.Vid, data.MachineName, data.SubjectName, request.Start.Value, request.End.Value);
                return await Result<GetAllDetailMachineEnergyConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }

            var defaultData = await _defaultRepository.GetAllDetailMachineEnergyConsumptionDefault(data.Vid, data.MachineName, data.SubjectName);
            return await Result<GetAllDetailMachineEnergyConsumptionDto>.SuccessAsync(defaultData, "Successfully fetch data");
        }
    }
}