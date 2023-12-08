using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.FrameNumberSubject.Queries.GetAllFrameNumber
{
     public record GetAllFrameNumberQuery : IRequest<Result<List<GetAllFrameNumberDto>>>;

    internal class GetAllFrameNumberQueryHandler : IRequestHandler<GetAllFrameNumberQuery, Result<List<GetAllFrameNumberDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllFrameNumberQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllFrameNumberDto>>> Handle(GetAllFrameNumberQuery request, CancellationToken cancellationToken)
        {
            var subject = await _unitOfWork.Repository<FrameNumber>().Entities.Where(x => x.DeletedAt == null)
                .ProjectTo<GetAllFrameNumberDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return await Result<List<GetAllFrameNumberDto>>.SuccessAsync(subject, "Successfully fetch data");
        }
    }
}
