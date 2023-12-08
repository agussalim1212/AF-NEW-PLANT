using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage
{
   public record GetListQualityRobotScanImageQuery : IRequest<PaginatedResult<GetListQualityRobotScanImageDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }


        public GetListQualityRobotScanImageQuery() { }

        public GetListQualityRobotScanImageQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string types, DateTime startTime, DateTime endTime)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = types;
            start = startTime;
            end = endTime;
        }

        internal class GetListQualityRobotScanImageQueryHandler : IRequestHandler<GetListQualityRobotScanImageQuery, PaginatedResult<GetListQualityRobotScanImageDto>>
        {
            private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

            public GetListQualityRobotScanImageQueryHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
            {
                _detailAssyUnitRepository = detailAssyUnitRepository;
            }

            public async Task<PaginatedResult<GetListQualityRobotScanImageDto>> Handle(GetListQualityRobotScanImageQuery query, CancellationToken cancellationToken)
            {
                var data = await _detailAssyUnitRepository.GetAllListQualityRobotScanImage(query.machine_id,query.type,query.start,query.end);
                var dt = data.Where(c => query.search_term == null || query.search_term.ToLower() == c.DataBarcode.ToLower()
                || query.search_term.ToLower() == c.Status.ToLower()).ToList();
                return await dt.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
