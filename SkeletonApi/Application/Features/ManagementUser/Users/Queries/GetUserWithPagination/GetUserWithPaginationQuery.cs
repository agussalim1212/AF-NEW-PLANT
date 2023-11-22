using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ManagementUser.Users.Queries.GetUserWithPagination
{
    public record GetUserWithPaginationQuery : IRequest<PaginatedResult<GetUserWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetUserWithPaginationQuery() { }

        public GetUserWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }
    internal class GetUserWithPaginationQueryHandler : IRequestHandler<GetUserWithPaginationQuery, PaginatedResult<GetUserWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetUserWithPaginationDto>> Handle(GetUserWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Data<UserRole>().FindByCondition(x => x.User.DeletedAt == null).Where(j => (query.search_term == null) 
            || (query.search_term.ToLower() == j.User.UserName.ToLower())
            || (query.search_term.ToLower() == j.Role.Name.ToLower()) 
            || (query.search_term.ToLower() == j.User.Email.ToLower()))
            .Include(v => v.User).Include(p => p.Role)
            .GroupBy(p => new { p.User.Id, p.User.UserName, p.User.Email, p.User.PasswordHash, p.User.UpdatedAt })
            .Select(o => new GetUserWithPaginationDto
            {
                Id = o.Key.Id,
                UserName = o.Key.UserName,
                PasswordHash = o.Key.PasswordHash,
                Email = o.Key.Email,
                UpdatedAt = o.Key.UpdatedAt.Value.AddHours(7),
                Roles = o.Select(m => new Roles
                {
                    Name = m.Role.Name
                }).ToList(),
            })
                .ProjectTo<GetUserWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);

        }
    }
}
