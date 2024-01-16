using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine
{

    public record GetAllMachineInformationAssyWheelLineQuery : IRequest<Result<GetAllMachineInformationAssyWheelLineDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationAssyWheelLineQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetAllMachineInformationAssyWheelHandler : IRequestHandler<GetAllMachineInformationAssyWheelLineQuery, Result<GetAllMachineInformationAssyWheelLineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDetailAssyWheelLineRepository _detailAssyWheelLine;

        public GetAllMachineInformationAssyWheelHandler(IUnitOfWork unitOfWork, IMapper mapper, IDetailAssyWheelLineRepository detailAssyWheelLine)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _detailAssyWheelLine = detailAssyWheelLine;
        }

        public async Task<Result<GetAllMachineInformationAssyWheelLineDto>> Handle(GetAllMachineInformationAssyWheelLineQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailAssyWheelLine.GetAllMachineInformationAsync(query.MachineId);
            return await Result<GetAllMachineInformationAssyWheelLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
