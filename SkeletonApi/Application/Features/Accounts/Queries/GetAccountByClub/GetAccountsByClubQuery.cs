using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Accounts.Queries.GetAccountByClub
{

    public record GetAccountsByClubQuery : IRequest<Result<List<GetAccountsByClubDto>>>
    {
        public Guid ClubId { get; set; }

        public GetAccountsByClubQuery()
        {

        }

        public GetAccountsByClubQuery(Guid clubId)
        {
            ClubId = clubId;
        }
    }

    internal class GetAccountsByClubQueryHandler : IRequestHandler<GetAccountsByClubQuery, Result<List<GetAccountsByClubDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountsByClubQueryHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAccountsByClubDto>>> Handle(GetAccountsByClubQuery query, CancellationToken cancellationToken)
        {
            var entities = await _accountRepository.GetAccountsByClubAsync(query.ClubId);
            var accounts = _mapper.Map<List<GetAccountsByClubDto>>(entities);
            return await Result<List<GetAccountsByClubDto>>.SuccessAsync(accounts);
        }
    }
}
