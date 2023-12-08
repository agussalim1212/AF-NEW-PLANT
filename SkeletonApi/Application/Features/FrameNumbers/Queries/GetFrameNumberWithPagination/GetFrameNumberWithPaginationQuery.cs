using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumb.Queries.GetFrameNumberWithPagination
{
   public record GetFrameNumberWithPaginationQuery : IRequest<PaginatedResult<GetFrameNumberWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string search_term { get; set; }

        public GetFrameNumberWithPaginationQuery() { }

        public GetFrameNumberWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }
    internal class GetFrameNumberWithPaginationQueryHandler : IRequestHandler<GetFrameNumberWithPaginationQuery, PaginatedResult<GetFrameNumberWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFrameNumberWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetFrameNumberWithPaginationDto>> Handle(GetFrameNumberWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<FrameNumber>().FindByCondition(x => x.DeletedAt == null)
                   .OrderBy(x => x.Vid).Where(s => query.search_term == null || query.search_term.ToLower() == s.Vid.ToLower() 
                   || query.search_term.ToLower() == s.Name.ToLower()).Select(m => new GetFrameNumberWithPaginationDto
                   {
                       Id = m.Id,
                       Vid = m.Vid,
                       Name = m.Name,
                       UpdatedAt = m.UpdatedAt.Value.AddHours(7),
                   })
                   .ProjectTo<GetFrameNumberWithPaginationDto>(_mapper.ConfigurationProvider)
                   .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}
