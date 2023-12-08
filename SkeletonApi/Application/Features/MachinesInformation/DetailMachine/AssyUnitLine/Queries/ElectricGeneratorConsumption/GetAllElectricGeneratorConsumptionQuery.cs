using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption
{
    public record GetAllElectricGeneratorConsumptionQuery : IRequest<Result<GetAllElectricGeneratorConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllElectricGeneratorConsumptionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllElectricGeneratorConsumptionHandler : IRequestHandler<GetAllElectricGeneratorConsumptionQuery, Result<GetAllElectricGeneratorConsumptionDto>>
    {
        private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

        public GetAllElectricGeneratorConsumptionHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
        {
           _detailAssyUnitRepository = detailAssyUnitRepository;
        }


        public async Task<Result<GetAllElectricGeneratorConsumptionDto>> Handle(GetAllElectricGeneratorConsumptionQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailAssyUnitRepository.GetAllElectricGeneratorConsumption(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllElectricGeneratorConsumptionDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
