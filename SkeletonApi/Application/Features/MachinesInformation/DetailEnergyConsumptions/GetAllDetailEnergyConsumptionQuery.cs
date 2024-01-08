using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions
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
        private readonly IDetailAssyUnitRepository _repository;
 

        public GetAllDetailEnergyConsumptionQueryHandler(IDetailAssyUnitRepository repository)
        {
           _repository = repository;
        }


        public async Task<Result<List<GetAllDetailEnergyConsumptionDto>>> Handle(GetAllDetailEnergyConsumptionQuery query, CancellationToken cancellationToken)
        {
            var data = await _repository.GetAllEnergyConsumptionSummary(query.type, query.start, query.end);
            return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(data, "succesfully fetch data");
        }


    }
}
