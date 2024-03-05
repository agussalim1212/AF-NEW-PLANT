using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Accounts.Profiles.Queries.GetAllAccountsByUsername
{
    public record GetAllAccountsQuery : IRequest<Result<List<GetAllAccountsDto>>>
    {
        public string Username { get; set; }
        public GetAllAccountsQuery(string username)
        {
            Username = username;
        }
    }

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
            var Account = await _unitOfWork.Repository<Account>().FindByCondition(o => o.Username == query.Username).FirstOrDefaultAsync();
            if(Account == null)
            {
                var user = await _unitOfWork.Data<User>().FindByCondition(o => o.UserName == query.Username).Select(o => new GetAllAccountsDto
                {
                    Username = o.UserName,
                    Email = o.Email
                })
                .ProjectTo<GetAllAccountsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
                return await Result<List<GetAllAccountsDto>>.SuccessAsync(user, "Successfully fetch data");
            }
            else
            {
                var user = await _unitOfWork.Data<User>().FindByCondition(o => o.UserName == query.Username).Select(o => new GetAllAccountsDto
                {
                    Id = Account.Id,
                    Foto = Account.PhotoURL,
                    Username = o.UserName,
                    Email = o.Email
                })
                .ProjectTo<GetAllAccountsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
               return await Result<List<GetAllAccountsDto>>.SuccessAsync(user, "Successfully fetch data");
            }

        }
    }
}
