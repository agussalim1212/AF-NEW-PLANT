using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.Machines.Commands.CreateMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.CategoryMachine.Commands.CreateCategoryHasMachine
{
    public record CreateCategoryHasMachineCommand : IRequest<Result<CategoryMachineHasMachine>>, IMapFrom<CategoryMachineHasMachine>
    {
     
        [JsonPropertyName("category_id")]
        public Guid CategoryMachineId { get; set; }
        [JsonPropertyName("machine_id")]
        public List<Guid> MachineId { get; set; }
    }
    internal class CreateCategoryHasMachineCommandHandler : IRequestHandler<CreateCategoryHasMachineCommand, Result<CategoryMachineHasMachine>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryHasMachineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CategoryMachineHasMachine>> Handle(CreateCategoryHasMachineCommand request, CancellationToken cancellationToken)
        {
            foreach (var machineId in request.MachineId)
            {
                var categoryMachines = await _unitOfWork.Repo<CategoryMachineHasMachine>().Entities.Where(x => request.CategoryMachineId == x.CategoryMachineId && machineId == x.MachineId).ToListAsync();

                if(categoryMachines.Count == 0)
                {

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
                return await Result<CategoryMachineHasMachine>.SuccessAsync(categoryMachine, "Category Has Machines Created");
                }
                else
                {
                 return await Result<CategoryMachineHasMachine>.FailureAsync("Category Has Machine Already Exist");
                }
            }
            return await Result<CategoryMachineHasMachine>.FailureAsync("CategoryId Or MachineId Not Found");
        }
    }
}
