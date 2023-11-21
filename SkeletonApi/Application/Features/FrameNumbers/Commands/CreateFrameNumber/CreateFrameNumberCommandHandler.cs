using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.FrameNumb.Commands.CreateFrameNumber
{
    internal class CreateFrameNumberCommandHandler : IRequestHandler<CreateFrameNumberRequest, Result<CreateFrameNumberResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateFrameNumberCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CreateFrameNumberResponseDto>> Handle(CreateFrameNumberRequest request, CancellationToken cancellationToken)
        {
            var frameNumber = _mapper.Map<FrameNumber>(request);

            frameNumber.CreatedAt = DateTime.UtcNow;
            frameNumber.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<FrameNumber>().AddAsync(frameNumber);
            frameNumber.AddDomainEvent(new FrameNumberCreatedEvent(frameNumber));
            await _unitOfWork.Save(cancellationToken);
            var frameNumberResponse = _mapper.Map<CreateFrameNumberResponseDto>(frameNumber);
            return await Result<CreateFrameNumberResponseDto>.SuccessAsync(frameNumberResponse, "Frame Number Created.");

        }
    }
}
