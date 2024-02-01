using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine
{
    public record GetAllAirConsumptionAssyWheelLineQuery : IRequest<Result<GetAllAirConsumptionAssyWheelLineDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllAirConsumptionAssyWheelLineQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllAirConsumptionAssyWheelHandler : IRequestHandler<GetAllAirConsumptionAssyWheelLineQuery, Result<GetAllAirConsumptionAssyWheelLineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDetailAssyWheelLineRepository _detailAssyWheelLine;

        public GetAllAirConsumptionAssyWheelHandler(IUnitOfWork unitOfWork, IMapper mapper, IDetailAssyWheelLineRepository detailAssyWheelLine)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _detailAssyWheelLine = detailAssyWheelLine;
        }


        public async Task<Result<GetAllAirConsumptionAssyWheelLineDto>> Handle(GetAllAirConsumptionAssyWheelLineQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailAssyWheelLine.GetAllAirConsumption(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllAirConsumptionAssyWheelLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
