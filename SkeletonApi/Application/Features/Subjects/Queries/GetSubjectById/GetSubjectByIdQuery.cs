using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectById
{
    public record GetSubjectByIdQuery : IRequest<Result<GetSubjectByIdDto>>
    {
        public Guid Id { get; set; }

        public GetSubjectByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, Result<GetSubjectByIdDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubjectByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetSubjectByIdDto>> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Repository<Subject>().GetByIdAsync(request.Id);

            if (entity != null)
            {
                if (entity.DeletedAt != null)
                    return await Result<GetSubjectByIdDto>.FailureAsync("Subject has been deleted");

                var player = _mapper.Map<GetSubjectByIdDto>(entity);
                return await Result<GetSubjectByIdDto>.SuccessAsync(player);
            }
            return await Result<GetSubjectByIdDto>.FailureAsync("Subject not found");
        }
    }
}