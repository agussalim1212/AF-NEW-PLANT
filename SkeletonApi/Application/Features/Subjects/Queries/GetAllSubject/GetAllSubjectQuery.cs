using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject
{
    public record GetAllSubjectQuery : IRequest<Result<List<GetAllSubjectDto>>>;

    internal class GetAllSubjectQueryHandler : IRequestHandler<GetAllSubjectQuery, Result<List<GetAllSubjectDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllSubjectQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllSubjectDto>>> Handle(GetAllSubjectQuery request, CancellationToken cancellationToken)
        {
            var subject = await _unitOfWork.Repository<Subject>().Entities.Where(x => x.DeletedAt == null)
                
                .Select(o => new GetAllSubjectDto
                {
                    Id = o.Id,
                    Vid = o.Vid
                })
                .ProjectTo<GetAllSubjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return await Result<List<GetAllSubjectDto>>.SuccessAsync(subject, "Successfully fetch data");
        }
    }
}