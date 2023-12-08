using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination
{
    public record GetSubjectWithPaginationQuery : IRequest<PaginatedResult<GetSubjectWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetSubjectWithPaginationQuery() { }

        public GetSubjectWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }
    internal class GetSubjectWithPaginationQueryHandler : IRequestHandler<GetSubjectWithPaginationQuery, PaginatedResult<GetSubjectWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubjectWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetSubjectWithPaginationDto>> Handle(GetSubjectWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Subject>().Entities.Where(o => query.search_term == null 
            || query.search_term.ToLower() == o.Vid.ToLower() 
            || query.search_term.ToLower() == o.Subjects.ToLower())
                   .OrderBy(x => x.Vid)
                   .ProjectTo<GetSubjectWithPaginationDto>(_mapper.ConfigurationProvider)
                   .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                   
        }
    }
}
