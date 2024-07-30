using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Settings.Queries.GetSubjectByMachineId
{
    public record GetSubjectByMachineIdQuery : IRequest<Result<List<GetSubjectByMachineIdDto>>>
    {
        public Guid MachineId { get; set; }
        public GetSubjectByMachineIdQuery(Guid machineId)
        {
            MachineId = machineId;
        }
    }

    internal class GetSubjectByMachineIdQueryHandler : IRequestHandler<GetSubjectByMachineIdQuery, Result<List<GetSubjectByMachineIdDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubjectByMachineIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetSubjectByMachineIdDto>>> Handle(GetSubjectByMachineIdQuery request, CancellationToken cancellationToken)
        {
            var subject = await _unitOfWork.Repo<SubjectHasMachine>().FindByCondition(o => o.MachineId == request.MachineId).Include(m => m.Machine).Include(s => s.Subject)
                          .Select(x => new GetSubjectByMachineIdDto { Subject = x.Subject.Subjects })
                          .ProjectTo<GetSubjectByMachineIdDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return await Result<List<GetSubjectByMachineIdDto>>.SuccessAsync(subject, "Success");
        }
    }
}