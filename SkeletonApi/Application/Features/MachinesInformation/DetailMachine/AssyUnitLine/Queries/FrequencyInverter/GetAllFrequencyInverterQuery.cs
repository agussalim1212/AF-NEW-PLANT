using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.FrequencyInverter
{
    public record GetAllFrequencyInverterQuery : IRequest<Result<GetAllFrequencyInverterDto>>
    {
        public Guid machine_id { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }

        public GetAllFrequencyInverterQuery(Guid MachineId, string Type, DateTime Start, DateTime End)
        {
            machine_id = MachineId;
            type = Type;
            start = Start;
            end = End;
        }

    }
    internal class GetAllMachineInformationHandler : IRequestHandler<GetAllFrequencyInverterQuery, Result<GetAllFrequencyInverterDto>>
    {
        private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

        public GetAllMachineInformationHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
        {
            _detailAssyUnitRepository = detailAssyUnitRepository;
        }

        public async Task<Result<GetAllFrequencyInverterDto>> Handle(GetAllFrequencyInverterQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailAssyUnitRepository.GetAllFrequencyInverter(query.machine_id, query.type, query.start, query.end);
            return await Result<GetAllFrequencyInverterDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

