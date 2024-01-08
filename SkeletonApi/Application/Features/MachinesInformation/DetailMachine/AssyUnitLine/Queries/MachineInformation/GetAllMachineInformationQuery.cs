using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation
{
    public record GetAllMachineInformationQuery : IRequest<Result<GetAllMachineInformationDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
        internal class GetAllMachineInformationHandler : IRequestHandler<GetAllMachineInformationQuery, Result<GetAllMachineInformationDto>>
        {
            private readonly IDetailAssyUnitRepository _repository;

            public GetAllMachineInformationHandler(IDetailAssyUnitRepository repository)
            {
                _repository = repository;
            }

            public async Task<Result<GetAllMachineInformationDto>> Handle(GetAllMachineInformationQuery query, CancellationToken cancellationToken)
            {
                var data = await _repository.GetAllMachineInformationAsync(query.MachineId);
                return await Result<GetAllMachineInformationDto>.SuccessAsync(data, "Successfully fetch data");
            }

        }

}


