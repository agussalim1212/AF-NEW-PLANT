using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumberSubject.Commands.DeleteFrameNumberHasSubject
{
    public record DeleteFrameNumberHasSubjectCommand : IRequest<Result<Guid>>, IMapFrom<FrameNumberHasSubjects>
    {
        public Guid Id { get; set; }

        public DeleteFrameNumberHasSubjectCommand()
        {

        }
        public DeleteFrameNumberHasSubjectCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteFrameNumberHasSubjectHandler : IRequestHandler<DeleteFrameNumberHasSubjectCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFrameNumberHasSubjectHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteFrameNumberHasSubjectCommand request, CancellationToken cancellationToken)
        {
            var FrameNumberSubjects = await _unitOfWork.Repo<FrameNumberHasSubjects>().Entities.Where(x => request.Id == x.FrameNumberId).ToListAsync();

            if (FrameNumberSubjects.Count != 0)
            {
                foreach (var cM in FrameNumberSubjects)
                {
                    await _unitOfWork.Repo<FrameNumberHasSubjects>().DeleteAsync(cM);
                    cM.AddDomainEvent(new FrameNumberHasSubjectDeletedEvent(cM));
                    await _unitOfWork.Save(cancellationToken);
                }
                return await Result<Guid>.SuccessAsync(request.Id, "Frame Number Has Subject Deleted");
            }

            return await Result<Guid>.FailureAsync("Frame Number Has Subject Not Found");
        }


    }
}
