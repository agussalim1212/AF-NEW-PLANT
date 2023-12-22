using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption
{
    public record GetAllAirConsumptionQuery : IRequest<Result<GetAllAirConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllAirConsumptionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }

    internal class GetAllAirConsumptionHandler : IRequestHandler<GetAllAirConsumptionQuery, Result<GetAllAirConsumptionDto>>
    {
        private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

        public GetAllAirConsumptionHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
        {
            _detailAssyUnitRepository = detailAssyUnitRepository;
        }


        public async Task<Result<GetAllAirConsumptionDto>> Handle(GetAllAirConsumptionQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailAssyUnitRepository.GetAllAirConsumption(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllAirConsumptionDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
