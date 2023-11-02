using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Subjects.Commands.CreateSubject
{
    public record CreateSubjectCommand : IRequest<Result<Subject>>, IMapFrom<Subject>
    {
        [JsonPropertyName("vid")]
        public string? Vid { get; set; }
        [JsonPropertyName("subject")]
        public string? Subjects { get; set; }
    }

    internal class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Result<Subject>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public CreateSubjectCommandHandler(IUnitOfWork unitOfWork, ISubjectRepository subjectRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<Result<Subject>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = new Subject()
            {
                Vid = request.Vid,
                Subjects = request.Subjects,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Subject>().AddAsync(subject);
            subject.AddDomainEvent(new SubjectCreatedEvent(subject));
            await _unitOfWork.Save(cancellationToken);
            return await Result<Subject>.SuccessAsync(subject, "Subject Created");
        }
    }
}