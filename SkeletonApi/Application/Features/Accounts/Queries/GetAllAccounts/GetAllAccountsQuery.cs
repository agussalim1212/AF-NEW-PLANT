using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Accounts.Queries.GetAccountByClub;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Accounts.Queries.GetAllAccounts
{
    public record GetAllAccountsQuery : IRequest<Result<List<GetAllAccountsDto>>>;

    internal class GetAllAccountQueryHandler : IRequestHandler<GetAllAccountsQuery, Result<List<GetAllAccountsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllAccountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllAccountsDto>>> Handle(GetAllAccountsQuery query, CancellationToken cancellationToken)
        {
            var accounts = await _unitOfWork.Repository<Account>().Entities
                   .ProjectTo<GetAllAccountsDto>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);

            return await Result<List<GetAllAccountsDto>>.SuccessAsync(accounts, "Successfully fetch data");
        }
    }
}
