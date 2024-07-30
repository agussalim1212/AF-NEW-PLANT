using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction
{
    public record GetAllTotalProductionQuery : IRequest<Result<GetAllTotalProductionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllTotalProductionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }

    internal class GetAllTotalProductionQueryHandler : IRequestHandler<GetAllTotalProductionQuery, Result<GetAllTotalProductionDto>>
    {
        private readonly IDefaultRepository _defaultRepository;
        private readonly IDayRepository _dayRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IMonthRepository _monthRepository;
        private readonly IYearRepository _yearRepository;
        private readonly IDetailMachineRepository _detailMachineRepository;

        public GetAllTotalProductionQueryHandler(IDefaultRepository defaultRepository, IDayRepository dayRepository, IWeekRepository weekRepository, IMonthRepository monthRepository, IYearRepository yearRepository, IDetailMachineRepository detailMachineRepository)
        {
            _defaultRepository = defaultRepository;
            _dayRepository = dayRepository;
            _weekRepository = weekRepository;
            _monthRepository = monthRepository;
            _yearRepository = yearRepository;
            _detailMachineRepository = detailMachineRepository;
        }

        public async Task<Result<GetAllTotalProductionDto>> Handle(GetAllTotalProductionQuery request, CancellationToken cancellationToken)
        {
            //untuk mendapatkan nama subject berdasarkan id machine dan vid yang mengandung produksi ok/ng
            var vidOK = await _detailMachineRepository.GetSubjectAsync(request.MachineId, "PRODUKSI-OK");
            var vidNG = await _detailMachineRepository.GetSubjectAsync(request.MachineId, "PRODUKSI-NG");

            //jika type == day maka data akan di arahkan ke proses filtering day yang berada di SkeletonApi.Application.Interfaces.Repositories.Filtering, didalam repositories tsb 
            //terdapat method untuk filtering tergantung type yang di pilih
            if (request.Type == "day")
            {
                var data = await _dayRepository.GetAllTotalProductionDay(request.MachineId, vidOK.Vid, vidNG.Vid, vidOK.MachineName, request.Start, request.End);
                return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
            }
            else if (request.Type == "week")
            {
                var data = await _weekRepository.GetAllTotalProductionWeek(request.MachineId, vidOK.Vid, vidNG.Vid, vidOK.MachineName, request.Start, request.End);
                return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
            }
            else if (request.Type == "month")
            {
                var data = await _monthRepository.GetAllTotalProductionMonth(request.MachineId, vidOK.Vid, vidNG.Vid, vidOK.MachineName, request.Start, request.End);
                return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
            }
            else if (request.Type == "year")
            {
                var data = await _yearRepository.GetAllTotalProductionYear(request.MachineId, vidOK.Vid, vidNG.Vid, vidOK.MachineName, request.Start, request.End);
                return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
            }
            else
            {
                var data = await _defaultRepository.GetAllTotalProductionDefault(request.MachineId, vidOK.Vid, vidNG.Vid, vidOK.MachineName);
                return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
            }
        }
    }
}