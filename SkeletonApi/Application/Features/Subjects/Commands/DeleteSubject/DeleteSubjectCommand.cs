using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Subjects.Commands.DeleteSubject
{
    public record DeleteSubjectCommand : IRequest<Result<Guid>>, IMapFrom<Subject>
    {
        public Guid Id { get; set; }

        public DeleteSubjectCommand()
        {
        }

        public DeleteSubjectCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteSubjectCommandHandle : IRequestHandler<DeleteSubjectCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubjectCommandHandle(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = await _unitOfWork.Repository<Subject>().GetByIdAsync(request.Id);

            if (subject != null)
            {
                subject.DeletedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Subject>().DeleteAsync(subject);
                subject.AddDomainEvent(new SubjectDeletedEvent(subject));
                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(subject.Id, "Subject Deleted");
            }

            return await Result<Guid>.FailureAsync("Subject not found");
        }
    }
}