using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Subjects.Commands.UpdateSubject
{
    public record UpdateSubjectCommand : IRequest<Result<Subject>>, IMapFrom<Subject>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("vid")]
        public string Vid { get; set; }
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
    }

    internal class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, Result<Subject>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSubjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Subject>> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = await _unitOfWork.Repository<Subject>().GetByIdAsync(request.Id);

            if (subject != null)
            {
                subject.Vid = request.Vid;
                subject.Subjects = request.Subject;
                subject.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Subject>().UpdateAsync(subject);
                subject.AddDomainEvent(new SubjectUpdatedEvent(subject));

                await _unitOfWork.Save(cancellationToken);
                return await Result<Subject>.SuccessAsync(subject, "Subject Updated");
            }
            return await Result<Subject>.FailureAsync("Subject not found");
        }
    }
}