using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination
{
    public record GetTreacibilityWithPaginationQuery : IRequest<PaginatedResult<GetTreacibilityWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }

        public GetTreacibilityWithPaginationQuery() { }

        public GetTreacibilityWithPaginationQuery(int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
        }
    }
    internal class GetTreacibilityWithPaginationQueryHandler : IRequestHandler<GetTreacibilityWithPaginationQuery, PaginatedResult<GetTreacibilityWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTreacibilityWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetTreacibilityWithPaginationDto>> Handle(GetTreacibilityWithPaginationQuery query, CancellationToken cancellationToken)
        {

            return await _unitOfWork.Data<EnginePart>().Entities
                     .OrderByDescending(x => x.DateTime)
                     .ProjectTo<GetTreacibilityWithPaginationDto>(_mapper.ConfigurationProvider)
                     .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);              
        }
    }
}
