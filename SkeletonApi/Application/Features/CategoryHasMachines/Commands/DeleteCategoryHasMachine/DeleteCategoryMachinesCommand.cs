using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.Machines.Commands.DeleteMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.DeleteCategoryHasMachine
{
    public record DeleteCategoryHasMachinesCommand : IRequest<Result<Guid>>, IMapFrom<CategoryMachineHasMachine>
    {
        public Guid Id { get; set; }

        public DeleteCategoryHasMachinesCommand()
        {

        }
        public DeleteCategoryHasMachinesCommand(Guid id)
        {
            Id = id;
        }
    }

   internal class DeleteCategoryHasMachinesHandler : IRequestHandler<DeleteCategoryHasMachinesCommand, Result<Guid>>
   {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHasMachinesHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteCategoryHasMachinesCommand request, CancellationToken cancellationToken)
        {
            var categoryMachines = await _unitOfWork.Repo<CategoryMachineHasMachine>().Entities.Where(x => request.Id == x.CategoryMachineId).ToListAsync();
            Console.WriteLine(categoryMachines);

            if (categoryMachines.Count != 0)
            {
                foreach (var cM in categoryMachines)
                {
                    await _unitOfWork.Repo<CategoryMachineHasMachine>().DeleteAsync(cM);
                    cM.AddDomainEvent(new CategoryHasMachinesDeletedEvent(cM));
                    await _unitOfWork.Save(cancellationToken);
                }
                return await Result<Guid>.SuccessAsync(request.Id, "Category Has Machines Deleted");
            }

            return await Result<Guid>.FailureAsync("Category Has Machines not found");
        }


    }

}
