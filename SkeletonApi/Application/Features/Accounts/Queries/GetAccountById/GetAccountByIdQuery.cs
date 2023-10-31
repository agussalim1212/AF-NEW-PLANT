using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts.Queries.GetAccountById
{
    //internal class GetAccountByIdQuery
    //{
    //}
    public record GetAccountByIdQuery : IRequest<Result<GetAccountByIdDto>>
    {
        public Guid Id { get; set; }

        public GetAccountByIdQuery()
        {
        }

        public GetAccountByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<GetAccountByIdDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAccountByIdDto>> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Repository<Account>().GetByIdAsync(query.Id);
            var player = _mapper.Map<GetAccountByIdDto>(entity);
            return await Result<GetAccountByIdDto>.SuccessAsync(player);
        }
    }
}