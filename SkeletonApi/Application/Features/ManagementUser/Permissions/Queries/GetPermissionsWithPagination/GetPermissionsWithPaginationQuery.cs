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
            return await _unitOfWork.Data<Role>().FindByCondition(g => g.DeletedAt == null).Include(o => o.Permissions)
           .Select(m => new GetPermissionsWithPaginationDto
           {
               //UserName = m.Key.UserName,
               //Email = m.Key.Email,
               RoleName = m.Name,
               //Permissions = m.Permissions.Select(d => d.ClaimType),
               //UpdateAt = m.Key.UpdatedAt.Value.AddHours(7)
           })
                
            .ProjectTo<GetPermissionsWithPaginationDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);

            //return await _unitOfWork.Data<Role>().FindByCondition(x => x.DeletedAt == null).Where(j => (query.search_term == null)
            //|| (query.search_term.ToLower() == j.Name.ToLower())).Select(c => new GetRolesWithPaginationDto
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    UpdateAt = c.UpdatedAt.Value.AddHours(7)
            //}).ProjectTo<GetRolesWithPaginationDto>(_mapper.ConfigurationProvider)
            //.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);


        }
    }
}
