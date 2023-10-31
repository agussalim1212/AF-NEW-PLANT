using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Accounts.Queries.GetAllAccounts;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Machines.Queries.GetAllMachines
{
    public record GetAllMachinesQuery : IRequest<Result<List<GetAllMachinesDto>>>;


    internal class GetAllMachineQueryHandler : IRequestHandler<GetAllMachinesQuery, Result<List<GetAllMachinesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMachineQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllMachinesDto>>> Handle(GetAllMachinesQuery query, CancellationToken cancellationToken)
        {
            var machines = await _unitOfWork.Repository<Machine>().Entities
                            .ProjectTo<GetAllMachinesDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

            return await Result<List<GetAllMachinesDto>>.SuccessAsync(machines, "Successfully fetch data");
        }
    }

}
