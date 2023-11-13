using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Commands.CreateSubjectHasMachine
{
    public record CreateSubjectHasMachineCommand : IRequest<Result<SubjectHasMachine>>, IMapFrom<SubjectHasMachine>
    {

        [JsonPropertyName("machine_id")]
        public Guid MachineId { get; set; }
        [JsonPropertyName("subject_id")]
        public List<Guid> SubjectId { get; set; }
    }
    internal class CreateSubjectHasMachineCommandHandler : IRequestHandler<CreateSubjectHasMachineCommand, Result<SubjectHasMachine>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSubjectHasMachineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<SubjectHasMachine>> Handle(CreateSubjectHasMachineCommand request, CancellationToken cancellationToken)
        {
        
        var subjectMachine = new SubjectHasMachine()
        {
            MachineId = request.MachineId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        foreach (var subId in request.SubjectId)
        {
            var subjectMachines = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Where(x => request.MachineId == x.MachineId && subId == x.SubjectId).ToListAsync();

            if(subjectMachine == null)
            {
                subjectMachine.SubjectId = subId;
                await _unitOfWork.Repo<SubjectHasMachine>().AddAsync(subjectMachine);
                subjectMachine.AddDomainEvent(new SubjectCreatedEvent(subjectMachine));
                await _unitOfWork.Save(cancellationToken);
            }
            else
            {
                return await Result<SubjectHasMachine>.FailureAsync("Subject Has Machines Already Exist");
            }
        }
        return await Result<SubjectHasMachine>.SuccessAsync(subjectMachine, "Subject Has Machines Created");
        }               
    }
}
