using MediatR;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine
{
    public record GetAllEnergyConsumptionGensubQuery : IRequest<Result<GetAllEnergyConsumptionGensubDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionGensubQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllEnergyConsumptionGensubHandler : IRequestHandler<GetAllEnergyConsumptionGensubQuery, Result<GetAllEnergyConsumptionGensubDto>>
    {
        private readonly IDetailGensubRespository _detailGensubRespository;

        public GetAllEnergyConsumptionGensubHandler(IDetailGensubRespository detailGensubRespository)
        {
            _detailGensubRespository = detailGensubRespository;
        }


        public async Task<Result<GetAllEnergyConsumptionGensubDto>> Handle(GetAllEnergyConsumptionGensubQuery query, CancellationToken cancellationToken)
        {
            var data = await  _detailGensubRespository.GetAllEnergyConsumptionGensubDto(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllEnergyConsumptionGensubDto>.SuccessAsync(data,"Successfully fetch data");
        }

    }
}

