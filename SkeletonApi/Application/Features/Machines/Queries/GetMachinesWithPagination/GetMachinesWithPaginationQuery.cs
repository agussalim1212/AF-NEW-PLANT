using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Machines.Queries.GetAllMachines
{
    public record GetMachinesWithPaginationQuery : IRequest<PaginatedResult<GetMachinesWithPaginationDto>>
    {
        //[JsonPropertyName("page_number")] tidak fungsi
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }


        public GetMachinesWithPaginationQuery() { }

        public GetMachinesWithPaginationQuery(int pageNumber, int pageSize, string searchTerm) 
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            

        }
    }
    internal class GetMachinesWithPaginationQueryHandler : IRequestHandler<GetMachinesWithPaginationQuery, PaginatedResult<GetMachinesWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMachinesWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetMachinesWithPaginationDto>> Handle(GetMachinesWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Machine>().FindByCondition(x => x.DeletedAt == null)
            .Where(o => (query.search_term == null) || (query.search_term.ToLower() == o.Name.ToLower()))
            .OrderBy(x => x.UpdatedAt)
            .ProjectTo<GetMachinesWithPaginationDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
        


       
    }
}
