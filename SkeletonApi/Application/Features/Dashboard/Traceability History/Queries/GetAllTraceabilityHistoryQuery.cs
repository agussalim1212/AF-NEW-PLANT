using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Dashboard.Traceability_History.Queries
{
 
        public record GetAllTraceabilityHistoryQuery : IRequest<Result<List<GetAllTraceabilityHistoryDto>>>;
        internal class GetAllTraceabilityHistoryQueryHandler : IRequestHandler<GetAllTraceabilityHistoryQuery, Result<List<GetAllTraceabilityHistoryDto>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
           
            public GetAllTraceabilityHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Result<List<GetAllTraceabilityHistoryDto>>> Handle(GetAllTraceabilityHistoryQuery query, CancellationToken cancellationToken)
            {
                var trc = await _unitOfWork.Data<EnginePart>().Entities.OrderByDescending(o => o.DateTime.AddHours(7)).Select(p => new GetAllTraceabilityHistoryDto
                {
                    EngineId = p.EngineId,
                    Status = p.Status,
                    DateTime = p.DateTime.AddHours(7)
                })
                .ProjectTo<GetAllTraceabilityHistoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
              
                return await Result<List<GetAllTraceabilityHistoryDto>>.SuccessAsync(trc, "Successfully fetch data");
            }
        }
    }

