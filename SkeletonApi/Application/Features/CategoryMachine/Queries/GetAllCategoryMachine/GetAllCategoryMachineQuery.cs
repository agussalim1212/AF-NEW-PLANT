using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Accounts.Queries.GetAllAccounts;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Queries.GetAllCategoryMachine
{
  
        public record GetAllCategoryMachineQuery : IRequest<Result<List<GetAllCategoryMachineDto>>>;

        internal class GetAllCategoryMachineQueryHandler : IRequestHandler<GetAllCategoryMachineQuery, Result<List<GetAllCategoryMachineDto>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoryMachineQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }


        public async Task<Result<List<GetAllCategoryMachineDto>>> Handle(GetAllCategoryMachineQuery query, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<CategoryMachines>().Entities.Where(x => x.DeletedAt == null)
                .ProjectTo<GetAllCategoryMachineDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return await Result<List<GetAllCategoryMachineDto>>.SuccessAsync(category, "Successfully fetch data");
        }

    }
}

