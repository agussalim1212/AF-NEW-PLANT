using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

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
            var category = await _unitOfWork.Repo<CategoryMachineHasMachine>().Entities.Include(c => c.Machine).Include(v => v.CategoryMachine).Select(g => new GetAllCategoryMachineDto
            {
                Id = g.MachineId,
                Name = g.Machine.Name,
                CategoryName = g.CategoryMachine.Name
            })
                .ProjectTo<GetAllCategoryMachineDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return await Result<List<GetAllCategoryMachineDto>>.SuccessAsync(category, "Successfully fetch data");
        }

    }
}

