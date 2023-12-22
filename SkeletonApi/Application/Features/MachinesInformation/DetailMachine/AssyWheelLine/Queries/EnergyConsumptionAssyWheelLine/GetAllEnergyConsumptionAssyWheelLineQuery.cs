using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine
{
    public record GetAllEnergyConsumptionAssyWheelLineQuery : IRequest<Result<GetAllEnergyConsumptionAssyWheelLineDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionAssyWheelLineQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllEnergyConsumptionAssyWheelLineHandler : IRequestHandler<GetAllEnergyConsumptionAssyWheelLineQuery, Result<GetAllEnergyConsumptionAssyWheelLineDto>>
    {
        private readonly IDetailAssyWheelLineRepository _repository;

        public GetAllEnergyConsumptionAssyWheelLineHandler(IDetailAssyWheelLineRepository detailAssyWheelLineRepository)
        {
           _repository = detailAssyWheelLineRepository;
        }


        public async Task<Result<GetAllEnergyConsumptionAssyWheelLineDto>> Handle(GetAllEnergyConsumptionAssyWheelLineQuery query, CancellationToken cancellationToken)
        {
            var data = await _repository.GetAllEnergyConsumption(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllEnergyConsumptionAssyWheelLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

