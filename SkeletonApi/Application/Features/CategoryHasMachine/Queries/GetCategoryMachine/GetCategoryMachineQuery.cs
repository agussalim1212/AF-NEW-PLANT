using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetAllCategoryMachine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryHasMachine.Queries.GetCategoryMachine
{
    public record GetCategoryMachineQuery : IRequest<Result<List<GetCategoryMachineDto>>>;

    internal class GetCategoryMachineQueryHandler : IRequestHandler<GetCategoryMachineQuery, Result<List<GetCategoryMachineDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryMachineQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<List<GetCategoryMachineDto>>> Handle(GetCategoryMachineQuery query, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<CategoryMachines>().Entities.Select(g => new GetCategoryMachineDto
            {
                Id = g.Id,
                Name = g.Name,
            })
            .ProjectTo<GetCategoryMachineDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

            return await Result<List<GetCategoryMachineDto>>.SuccessAsync(category, "Successfully fetch data");
        }

    }
}
