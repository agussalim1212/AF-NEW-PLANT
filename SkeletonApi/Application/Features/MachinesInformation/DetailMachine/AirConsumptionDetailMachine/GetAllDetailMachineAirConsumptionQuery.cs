using MediatR;
using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine
{
    //untuk semua machine yang mempunyai air consumption
    public record GetAllDetailMachineAirConsumptionQuery : IRequest<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string View { get; set; }
        public string Vid { get; set; }
        public GetAllDetailMachineAirConsumptionQuery(Guid machineId, string type, DateTime startTime, DateTime endTime, string view, string vid)
        {
            MachineId = machineId;
            Type = type;
            Start = startTime;
            End = endTime;
            View = view;
            Vid = vid;
        }
    }

    internal class GetAllDetailMachineAirConsumptionQueryHandler : IRequestHandler<GetAllDetailMachineAirConsumptionQuery, Result<GetAllDetailMachineAirAndElectricConsumptionDto>>
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

        public async Task<Result<GetAllDetailMachineAirAndElectricConsumptionDto>> Handle(GetAllDetailMachineAirConsumptionQuery request, CancellationToken cancellationToken)
        {
            //untuk mendapatkan nama subject berdasarkan id machine dan vid
            var data = await _repository.GetSubjectAsync(request.MachineId, request.Vid);

            //jika type == day maka data akan di arahkan ke proses filtering day yang berada di SkeletonApi.Application.Interfaces.Repositories.Filtering, didalam repositories tsb 
            //terdapat method untuk filtering tergantung type yang di pilih
            if (request.Type == "day")
            {
                var dt = await _dayRepository.GetAllDetailMachineAirAndElectricConsumptionAsync(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "week")
            {
                var dt = await _weekRepository.GetAllDetailMachineAirAndElectricConsumptionWeek(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "month")
            {
                var dt = await _monthRepository.GetAllDetailMachineAirAndElectricConsumptionMonth(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }
            else if (request.Type == "year")
            {
                var dt = await _yearRepository.GetAllDetailMachineAirAndElectricConsumptionYear(request.View, data.Vid, data.MachineName, data.SubjectName, request.Start, request.End);
                return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(dt, "Successfully fetch data");
            }

            var defaultData = await _defaultRepository.GetAllDetailMachineAirAndElectricConsumptionDefault(request.View, data.Vid, data.MachineName, data.SubjectName);
            return await Result<GetAllDetailMachineAirAndElectricConsumptionDto>.SuccessAsync(defaultData, "Successfully fetch data");
        }
    }
}