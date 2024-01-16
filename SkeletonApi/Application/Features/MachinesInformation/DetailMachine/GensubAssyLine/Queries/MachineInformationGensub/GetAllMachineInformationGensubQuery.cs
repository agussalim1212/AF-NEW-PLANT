using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.MachineInformation
{
     public record GetAllMachineInformationGensubQuery : IRequest<Result<GetAllMachineInformationGensubDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationGensubQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetAllMachineInformationGensubHandler : IRequestHandler<GetAllMachineInformationGensubQuery, Result<GetAllMachineInformationGensubDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDetailGensubRespository _gensubRespository;

        public GetAllMachineInformationGensubHandler(IUnitOfWork unitOfWork, IMapper mapper, IDetailGensubRespository gensubRespository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _gensubRespository = gensubRespository;
        }

        public async Task<Result<GetAllMachineInformationGensubDto>> Handle(GetAllMachineInformationGensubQuery query, CancellationToken cancellationToken)
        {
            var data = await _gensubRespository.GetAllMachineInformationAsync(query.MachineId);
            return await Result<GetAllMachineInformationGensubDto>.SuccessAsync(data, "Successfully fetch data");
        }
    }
}
