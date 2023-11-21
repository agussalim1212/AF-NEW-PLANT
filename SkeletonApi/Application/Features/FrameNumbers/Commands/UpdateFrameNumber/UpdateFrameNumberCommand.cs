using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.FrameNumb.Commands.UpdateFrameNumber
{
    internal class UpdateFrameNumberCommand : IRequestHandler<UpdateFrameNumberRequest, Result<FrameNumber>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateFrameNumberCommand(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FrameNumber>> Handle(UpdateFrameNumberRequest request, CancellationToken cancellationToken)
        {
            var frameNumber = await _unitOfWork.Repository<FrameNumber>().GetByIdAsync(request.Id);
            Console.WriteLine(frameNumber);
            if (frameNumber != null)
            {
                frameNumber.Vid = request.Vid;
                frameNumber.Name = request.Name;
                frameNumber.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<FrameNumber>().UpdateAsync(frameNumber);
                frameNumber.AddDomainEvent(new FrameNumberUpdateEvent(frameNumber));

                await _unitOfWork.Save(cancellationToken);
                return await Result<FrameNumber>.SuccessAsync(frameNumber, "Frame Number Updated");
            }
            return await Result<FrameNumber>.FailureAsync("Frame Number Not Found");
        }
    }
}
