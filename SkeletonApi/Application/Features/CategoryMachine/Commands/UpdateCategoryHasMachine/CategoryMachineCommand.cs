using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Machines.Commands.UpdateMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.UpdateCategoryHasMachine
{
    public record UpdateCategoryHasMachinesCommand : IRequest<Result<CategoryMachineHasMachine>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
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

                var categoryMachines = await _unitOfWork.Repository<CategoryMachineHasMachine>().FindByCondition(x => x.CategoryMachineId.Equals(request.CategoryMachineId), trackChanges: true).ToListAsync();
                Console.WriteLine(categoryMachines);
            //    if (categoryMachines != null)
            //    {
            //        foreach(var machine in request.MachineId)
            //        {
                   
            //        categoryMachines.CategoryMachineId = request.CategoryMachineId;
            //        categoryMachines.MachineId = machine;
            //        categoryMachines.UpdatedAt = DateTime.UtcNow;
            //        await _unitOfWork.Repository<CategoryMachineHasMachine>().UpdateAsync(categoryMachines);
            //        categoryMachines.AddDomainEvent(new CategoryMachineUpdateEvent(categoryMachines));
            //        await _unitOfWork.Save(cancellationToken);

            //        }
            //        return await Result<CategoryMachineHasMachine>.SuccessAsync(categoryMachines, "Category Machines Updated");
                
            //}
            return await Result<CategoryMachineHasMachine>.FailureAsync("Category Machine Not Found");
        }
    }
}
