using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetRoleWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetActivityUserWithPagination
{
     public record GetActivityUserWithPaginationQuery : IRequest<PaginatedResult<GetActivityUserWithPaginationDto>>
     {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetActivityUserWithPaginationQuery() { }

        public GetActivityUserWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
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
            return await _unitOfWork.Data<ActivityUser>().Entities.Where(j => (query.search_term == null)
            || (query.search_term.ToLower() == j.LogType.ToLower()) || (query.search_term.ToLower() == j.UserName.ToLower()))
            .Select(c => new GetActivityUserWithPaginationDto
            {
                Id = c.Id,
                UserName = c.UserName,
                LogType = c.LogType,
                Datetime = c.DateTime.AddHours(7)
            }).ProjectTo<GetActivityUserWithPaginationDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);

        }
    }
}
