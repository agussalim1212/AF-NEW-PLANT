using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.Accounts.Queries.GetAccountsWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Accounts.Queries.GetAccountWithPagination
{
    public record GetAccountsWithPaginationQuery : IRequest<PaginatedResult<GetAccountsWithPaginationDto>>
    {
        
        public int page_number { get; set; }
        
        public int page_size { get; set; }

        public GetAccountsWithPaginationQuery() { }

        public GetAccountsWithPaginationQuery(int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
        }
    }

    internal class GetAccountsWithPaginationQueryHandler : IRequestHandler<GetAccountsWithPaginationQuery, PaginatedResult<GetAccountsWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountsWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetAccountsWithPaginationDto>> Handle(GetAccountsWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Account>().Entities
                   .OrderBy(x => x.Name)
                   .ProjectTo<GetAccountsWithPaginationDto>(_mapper.ConfigurationProvider)
                   .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}
