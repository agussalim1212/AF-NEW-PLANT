using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine
{
      public record GetListQualityMainLineQuery : IRequest<PaginatedResult<GetListQualityMainLineDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public GetListQualityMainLineQuery() { }

        public GetListQualityMainLineQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string types, DateTime startTime, DateTime endTime)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = types;
            start = startTime;
            end = endTime;
        }

        internal class GetListQualityMainLineQueryHandler : IRequestHandler<GetListQualityMainLineQuery, PaginatedResult<GetListQualityMainLineDto>>
        {
            private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

            public GetListQualityMainLineQueryHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
            {
              _detailAssyUnitRepository = detailAssyUnitRepository;
            }

            public async Task<PaginatedResult<GetListQualityMainLineDto>> Handle(GetListQualityMainLineQuery query, CancellationToken cancellationToken)
            {
                var data = await _detailAssyUnitRepository.GetAllListQualityMainLine(query.machine_id,query.type,query.start,query.end);
                var dt = data.Where(c => query.search_term == null || query.search_term == c.FrqInverter.ToString()
                || query.search_term == c.DurationStop.ToString()).ToList();
                return await dt.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
