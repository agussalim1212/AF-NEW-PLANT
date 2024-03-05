using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.CategoryMachine.Commands.CreateCategoryHasMachine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Commands.UpdateFrameNumberHasSubject
{
   public record UpdateFrameNumberHasSubjectCommand : IRequest<Result<FrameNumberHasSubjects>>
    {

        [JsonPropertyName("frame_number_id")]
        public Guid FrameNumberId { get; set; }
        [JsonPropertyName("subject_id")]
        public List<Guid> SubjectId { get; set; }

    }

    internal class UpdateCategoryHasMachinesCommandHandler : IRequestHandler<UpdateFrameNumberHasSubjectCommand, Result<FrameNumberHasSubjects>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCategoryHasMachinesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FrameNumberHasSubjects>> Handle(UpdateFrameNumberHasSubjectCommand request, CancellationToken cancellationToken)
        {

            var frameNumberSubject = await _unitOfWork.Repo<FrameNumberHasSubjects>().Entities.Where(x => request.FrameNumberId == x.FrameNumberId).ToListAsync();
          
            if (frameNumberSubject.Count != 0)
            {

                foreach (var cM in frameNumberSubject)
                {
                    await _unitOfWork.Repo<FrameNumberHasSubjects>().DeleteAsync(cM);
                }

                var frameNumber = new FrameNumberHasSubjects()
                {
                    FrameNumberId = request.FrameNumberId,
                    UpdatedAt = DateTime.UtcNow,
                };

                foreach (var subject_id in request.SubjectId)
                {
                    frameNumber.SubjectId = subject_id;
                    await _unitOfWork.Repo<FrameNumberHasSubjects>().AddAsync(frameNumber);
                    frameNumber.AddDomainEvent(new FrameNumberSubjectUpdateEvent(frameNumber));
                    await _unitOfWork.Save(cancellationToken);
                }
                return await Result<FrameNumberHasSubjects>.SuccessAsync(frameNumber, "Frame Number Has Subject Updated");
            }
            else
            {
                return await Result<FrameNumberHasSubjects>.FailureAsync("Frame Number Has SUbject Not Found");
            }
        }
    }
}
