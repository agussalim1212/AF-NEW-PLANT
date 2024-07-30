using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetActivityUserWithPagination
{
    public record GetActivityUserWithPaginationQuery : IRequest<PaginatedResult<GetActivityUserWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }
        public DateTime? date_time { get; set; }
        public string? log_type { get; set; }
        public string? user_name { get; set; }

        public GetActivityUserWithPaginationQuery() { }

        public GetActivityUserWithPaginationQuery(string logType, string userName, DateTime dateTime, string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            date_time = dateTime;
            user_name = userName;
            log_type = logType;
        }
    }

    internal class GetActivityUserWithPaginationQueryHandler : IRequestHandler<GetActivityUserWithPaginationQuery, PaginatedResult<GetActivityUserWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetActivityUserWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetActivityUserWithPaginationDto>> Handle(GetActivityUserWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Data<ActivityUser>().Entities
            .Where(j => (query.search_term == null || query.search_term.ToLower() == j.LogType.ToLower() || query.search_term.ToLower() == j.UserName.ToLower())
            && (query.user_name == null || query.user_name.ToLower() == j.UserName.ToLower())
            && (query.log_type == null || query.log_type.ToLower() == j.LogType.ToLower())
            && (query.date_time == null || query.date_time == j.DateTime)).Select(g => new GetActivityUserWithPaginationDto
            {
                Id = g.Id,
                UserName = g.UserName,
                LogType = g.LogType,
                Datetime = g.DateTime
            })
            .OrderByDescending(o => o.Datetime).ProjectTo<GetActivityUserWithPaginationDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}