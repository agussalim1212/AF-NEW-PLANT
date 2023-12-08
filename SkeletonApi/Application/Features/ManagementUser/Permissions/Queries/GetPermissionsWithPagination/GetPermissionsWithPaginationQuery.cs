using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.ManagementUser.Permissions.Queries.GetRoleWithPagination;
using SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetRoleWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Linq;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Queries.GetPermissionsWithPagination
{
       public record GetPermissionsWithPaginationQuery : IRequest<PaginatedResult<GetPermissionsWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetPermissionsWithPaginationQuery() { }

        public GetPermissionsWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }
    internal class GetPermissionsWithPaginationQueryHandler : IRequestHandler<GetPermissionsWithPaginationQuery, PaginatedResult<GetPermissionsWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPermissionsWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetPermissionsWithPaginationDto>> Handle(GetPermissionsWithPaginationQuery query, CancellationToken cancellationToken)
        {


            var user = _unitOfWork.Data<UserRole>().Entities.Include(k => k.User).Include(m => m.Role).
            Where(j => j.Role.DeletedAt == null).Select(o => new { o.Role.Id, o.User.Email, o.User.UserName });

            return await _unitOfWork.Data<Permission>().Entities.Where(c => query.search_term == null
            || query.search_term.ToLower() == c.ClaimType.ToLower()
            || query.search_term.ToLower() == c.Role.Name.ToLower())
           .Include(k => k.Role)
           .GroupBy(n => new { n.Role.Name, n.ClaimType, n.UpdatedAt, n.Role.Id }).Select(m => new GetPermissionsWithPaginationDto
           {
               Id = m.Key.Id,
               UserName = user.Where( f => m.Key.Id == f.Id).Select(g => g.UserName).FirstOrDefault(),
               Email = user.Where(f => m.Key.Id == f.Id).Select(g => g.Email).FirstOrDefault(),
               RoleName = m.Key.Name,
               Permissions = m.Key.ClaimType,
               UpdateAt = m.Key.UpdatedAt.Value.AddHours(7)
           })

            .ProjectTo<GetPermissionsWithPaginationDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}
