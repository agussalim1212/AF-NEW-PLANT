using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.CategoryMachine.Commands.CreateCategoryHasMachine;
using SkeletonApi.Application.Features.SubjectHasMachines.Commands.CreateSubjectHasMachine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Commands.UpdateSubjectHasMachine_
{
    public record UpdateSubjectHasMachinesCommand : IRequest<Result<SubjectHasMachine>>
    {

        [JsonPropertyName("machine_id")]
        public Guid MachineId { get; set; }
        [JsonPropertyName("subject_id")]
        public List<Guid> SubjectId { get; set; }

    }

    internal class UpdateSubjectHasMachinesCommandHandler : IRequestHandler<UpdateSubjectHasMachinesCommand, Result<SubjectHasMachine>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSubjectHasMachinesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<SubjectHasMachine>> Handle(UpdateSubjectHasMachinesCommand request, CancellationToken cancellationToken)
        {

            var subjectMachines = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Where(x => request.MachineId == x.MachineId).ToListAsync();
            Console.WriteLine(subjectMachines);

            if (subjectMachines.Count != 0)
            {

                foreach (var sM in subjectMachines)
                {
                    await _unitOfWork.Repo<SubjectHasMachine>().DeleteAsync(sM);
                }

                var subjectMachine = new SubjectHasMachine()
                {
                    MachineId = request.MachineId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                foreach (var mc_id in request.SubjectId)
                {
                    subjectMachine.SubjectId = mc_id;
                    await _unitOfWork.Repo<SubjectHasMachine>().AddAsync(subjectMachine);
                    subjectMachine.AddDomainEvent(new SubjectCreatedEvent(subjectMachine));
                    await _unitOfWork.Save(cancellationToken);
                }
                return await Result<SubjectHasMachine>.SuccessAsync(subjectMachine, "Subject Machines Updated");
            }
            else
            {
                return await Result<SubjectHasMachine>.FailureAsync("Subject Machines Not Found");
            }
        }
    }
}
