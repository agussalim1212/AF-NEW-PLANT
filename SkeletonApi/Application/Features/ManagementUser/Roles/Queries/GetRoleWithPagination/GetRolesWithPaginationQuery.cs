using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetRoleWithPagination
{
    public record GetRolesWithPaginationQuery : IRequest<PaginatedResult<GetRolesWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetRolesWithPaginationQuery() { }

        public GetRolesWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }

    internal class GetRolesWithPaginationQueryHandler : IRequestHandler<GetRolesWithPaginationQuery, PaginatedResult<GetRolesWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRolesWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetRolesWithPaginationDto>> Handle(GetRolesWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Data<Role>().FindByCondition(x => x.DeletedAt == null).Where(j => (query.search_term == null)
            || (query.search_term.ToLower() == j.Name.ToLower())).Select(c => new GetRolesWithPaginationDto
            {
                Id = c.Id,
                Name = c.Name,
                UpdateAt = c.UpdatedAt.Value.AddHours(7)
            }).ProjectTo<GetRolesWithPaginationDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}