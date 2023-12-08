using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling
{
    public record GetListQualityCoolantFilingQuery : IRequest<PaginatedResult<GetListQualityCoolantFilingDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public GetListQualityCoolantFilingQuery() { }

        public GetListQualityCoolantFilingQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string types, DateTime startTime, DateTime endTime)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = types;
            start = startTime;
            end = endTime;
        }

        internal class GetListQualityCoolantFilingQueryHandler : IRequestHandler<GetListQualityCoolantFilingQuery, PaginatedResult<GetListQualityCoolantFilingDto>>
        {
            private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

            public GetListQualityCoolantFilingQueryHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
            {
               _detailAssyUnitRepository = detailAssyUnitRepository;
            }

            public async Task<PaginatedResult<GetListQualityCoolantFilingDto>> Handle(GetListQualityCoolantFilingQuery query, CancellationToken cancellationToken)
            {
                var data = await _detailAssyUnitRepository.GetAllListQualityCoolantFiling(query.machine_id, query.type, query.start, query.end);
                var dt = data.Where(o => query.search_term == null || query.search_term.ToLower() == o.DataBarcode.ToLower() || query.search_term.ToLower() == o.VolumeCoolant.ToString()).ToList();
                return await dt.ToPaginatedListAsync(query.page_number,query.page_size,cancellationToken);
            }
        }
    }
}
