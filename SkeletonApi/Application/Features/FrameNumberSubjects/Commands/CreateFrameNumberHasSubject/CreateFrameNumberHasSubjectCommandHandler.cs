using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.FrameNumberSubjects.Commands.CreateFrameNumberHasSubject
{
    public record CreateNumberHasSubjectCommand : IRequest<Result<FrameNumberHasSubjects>>, IMapFrom<FrameNumberHasSubjects>
    {
        [JsonPropertyName("frame_number_id")]
        public Guid FrameNumberId { get; set; }
        [JsonPropertyName("subject_id")]
        public List<Guid> SubjectId { get; set; }
    }

    internal class CreateSubjectHasMachineCommandHandler : IRequestHandler<CreateNumberHasSubjectCommand, Result<FrameNumberHasSubjects>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSubjectHasMachineCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FrameNumberHasSubjects>> Handle(CreateNumberHasSubjectCommand request, CancellationToken cancellationToken)
        {
            var subjectMachine = new FrameNumberHasSubjects()
            {
                FrameNumberId = request.FrameNumberId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            foreach (var subId in request.SubjectId)
            {
                var subjectMachines = await _unitOfWork.Repo<FrameNumberHasSubjects>().Entities.Where(x => request.FrameNumberId == x.FrameNumberId && subId == x.SubjectId).ToListAsync();

                if (subjectMachines.Count == 0)
                {
                    subjectMachine.SubjectId = subId;
                    await _unitOfWork.Repo<FrameNumberHasSubjects>().AddAsync(subjectMachine);
                    subjectMachine.AddDomainEvent(new FrameNumberHasSubjectCreatedEvent(subjectMachine));
                    await _unitOfWork.Save(cancellationToken);
                }
                else
                {
                    return await Result<FrameNumberHasSubjects>.FailureAsync("Subject Has Machines Already Exist");
                }
            }
            return await Result<FrameNumberHasSubjects>.SuccessAsync(subjectMachine, "Frame Number Has Subject Created");
        }
    }
}