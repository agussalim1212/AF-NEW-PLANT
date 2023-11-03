using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.CategoryMachine.Commands.CreateCategoryHasMachine;
using SkeletonApi.Application.Features.Machines.Commands.UpdateMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.UpdateCategoryHasMachine
{
    public record UpdateCategoryHasMachinesCommand : IRequest<Result<CategoryMachineHasMachine>>
    {
     
        [JsonPropertyName("category_id")]
        public Guid CategoryMachineId { get; set; }
        [JsonPropertyName("machine_id")]
        public List<Guid> MachineId { get; set; }

    }

    internal class UpdateCategoryHasMachinesCommandHandler : IRequestHandler<UpdateCategoryHasMachinesCommand, Result<CategoryMachineHasMachine>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCategoryHasMachinesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CategoryMachineHasMachine>> Handle(UpdateCategoryHasMachinesCommand request, CancellationToken cancellationToken)
        {

            var categoryMachines = await _unitOfWork.Repo<CategoryMachineHasMachine>().Entities.Where(x => request.CategoryMachineId == x.CategoryMachineId).ToListAsync();
            Console.WriteLine(categoryMachines);

            if (categoryMachines.Count != 0)
            {

                foreach (var cM in categoryMachines)
                {
                    await _unitOfWork.Repo<CategoryMachineHasMachine>().DeleteAsync(cM);
                }

                var categoryMachine = new CategoryMachineHasMachine()
                {
                    CategoryMachineId = request.CategoryMachineId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                foreach (var mc_id in request.MachineId)
                {
                    categoryMachine.MachineId = mc_id;
                    await _unitOfWork.Repo<CategoryMachineHasMachine>().AddAsync(categoryMachine);
                    categoryMachine.AddDomainEvent(new CategoryCreatedEvent(categoryMachine));
                    await _unitOfWork.Save(cancellationToken);
                }
                    return await Result<CategoryMachineHasMachine>.SuccessAsync(categoryMachine, "Category Machines Updated");
            }
            else
            {
              return await Result<CategoryMachineHasMachine>.FailureAsync("Category Machines Not Found");
            }
        }
    }
}
