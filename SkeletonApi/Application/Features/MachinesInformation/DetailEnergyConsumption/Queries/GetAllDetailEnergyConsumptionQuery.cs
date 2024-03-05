using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries
{
    public record GetAllDetailEnergyConsumptionQuery : IRequest<Result<List<GetAllDetailEnergyConsumptionDto>>>
    {
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public GetAllDetailEnergyConsumptionQuery(string Type, DateTime Start, DateTime End)
        {
            type = Type;
            start = Start;
            end = End;
        }
    }
    internal class GetAllDetailEnergyConsumptionQueryHandler : IRequestHandler<GetAllDetailEnergyConsumptionQuery, Result<List<GetAllDetailEnergyConsumptionDto>>>
    {
        private readonly IDayRepository _dayRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IMonthRepository _monthRepository;
        private readonly IYearRepository _yearRepository;
        private readonly IDefaultRepository _defaultRepository;


        public GetAllDetailEnergyConsumptionQueryHandler(IDayRepository dayRepository, IWeekRepository weekRepository, IMonthRepository monthRepository, IYearRepository yearRepository, IDefaultRepository defaultRepository)
        {
            _dayRepository = dayRepository;
            _weekRepository = weekRepository;
            _monthRepository = monthRepository;
            _yearRepository = yearRepository;
            _defaultRepository = defaultRepository; 
        }


        public async Task<Result<List<GetAllDetailEnergyConsumptionDto>>> Handle(GetAllDetailEnergyConsumptionQuery query, CancellationToken cancellationToken)
        {
            if (query.type == "day")
            {
                var data = await _dayRepository.GetAllEnergyConsumptionSummary(query.start, query.end);
                return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(data, "succesfully fetch data");
            }else if(query.type == "week")
            {
                var data = await _weekRepository.GetAllEnergyConsumptionSummary(query.start, query.end);
                return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(data, "succesfully fetch data");
            }
            else if(query.type == "month")
            {
                var data = await _monthRepository.GetAllEnergyConsumptionSummary(query.start, query.end);
                return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(data, "succesfully fetch data");
            }
            else if(query.type == "year")
            {
                var data = await _yearRepository.GetAllEnergyConsumptionSummary(query.start, query.end);
                return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(data, "succesfully fetch data");
            }
            var dt = await _defaultRepository.GetAllEnergyConsumptionSummary();
            return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(dt, "succesfully fetch data");
        }


    }
}
