using MediatR;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.TotalProduction
{
    public record GetAllTotalProductionGensubQuery : IRequest<Result<GetAllTotalProductionGensubDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllTotalProductionGensubQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }

    internal class GetAllTotalProductionGensubHandler : IRequestHandler<GetAllTotalProductionGensubQuery, Result<GetAllTotalProductionGensubDto>>
    {
        private readonly IDetailGensubRespository _detailGensubRespository;

        public GetAllTotalProductionGensubHandler(IDetailGensubRespository detailGensubRespository)
        {
          
           _detailGensubRespository = detailGensubRespository;
        }

        public async Task<Result<GetAllTotalProductionGensubDto>> Handle(GetAllTotalProductionGensubQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailGensubRespository.GetAllTotalProductionGensubDto(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllTotalProductionGensubDto>.SuccessAsync(data,"Successfully fetch data");
        }
    }
}
