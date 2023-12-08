using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetAllCategoryMachine;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectWithParam
{
    public record GetSubjectWithParamQuery : IRequest<Result<List<GetSubjectWithParamDto>>>
    {
        public Guid MachineId { get; set; }

        public GetSubjectWithParamQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetSubjectWithParamQueryHandler : IRequestHandler<GetSubjectWithParamQuery, Result<List<GetSubjectWithParamDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubjectWithParamQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetSubjectWithParamDto>>> Handle(GetSubjectWithParamQuery request, CancellationToken cancellationToken)
        {
            var subject = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(c => c.Subject).Include(v => v.Machine).Where(c => request.MachineId == c.Machine.Id).Select(g => new GetSubjectWithParamDto
            {
                Id = g.Subject.Id,
                Subject = g.Subject.Subjects,
            })
              .ProjectTo<GetSubjectWithParamDto>(_mapper.ConfigurationProvider)
              .ToListAsync(cancellationToken);

            return await Result<List<GetSubjectWithParamDto>>.SuccessAsync(subject, "Successfully fetch data");
        }
    }
}
