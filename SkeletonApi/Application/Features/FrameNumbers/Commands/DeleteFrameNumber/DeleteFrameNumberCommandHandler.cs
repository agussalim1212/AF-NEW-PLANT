using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumb.Commands.DeleteFrameNumber
{
   internal class DeleteFrameNumberCommandHandler : IRequestHandler<DeleteFrameNumberRequest, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFrameNumberCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteFrameNumberRequest request, CancellationToken cancellationToken)
        {
            var frameNumber = await _unitOfWork.Repository<FrameNumber>().GetByIdAsync(request.Id);
            if (frameNumber != null)
            {
               
                frameNumber.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<FrameNumber>().UpdateAsync(frameNumber);
                frameNumber.AddDomainEvent(new FrameNumberDeleteEvent(frameNumber));
                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(frameNumber.Id, "Frame Number Deleted.");
            }
            return await Result<Guid>.FailureAsync("Frame Number Not Found");
        }

    }
}
