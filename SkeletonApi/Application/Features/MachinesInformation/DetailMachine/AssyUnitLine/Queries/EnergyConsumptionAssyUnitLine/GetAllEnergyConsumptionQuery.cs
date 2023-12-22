using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine
{
    public record GetAllEnergyConsumptionQuery : IRequest<Result<GetAllEnergyConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }

    internal class GetAllEnergyConsumptionHandler : IRequestHandler<GetAllEnergyConsumptionQuery, Result<GetAllEnergyConsumptionDto>>
    {
        private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

        public GetAllEnergyConsumptionHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
        {
            _detailAssyUnitRepository = detailAssyUnitRepository;
        }

        public async Task<Result<GetAllEnergyConsumptionDto>> Handle(GetAllEnergyConsumptionQuery query, CancellationToken cancellationToken)
        {
           var data = await _detailAssyUnitRepository.GetAllEnergyConsumption(query.MachineId, query.Type, query.Start, query.End);
           return await Result<GetAllEnergyConsumptionDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

