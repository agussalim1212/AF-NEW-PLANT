using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetAllLogType
{
    public record GetAllLogTypeQuery : IRequest<Result<List<GetAllLogTypeDto>>>;

    internal class GetAllLogTypeQueryHandler : IRequestHandler<GetAllLogTypeQuery, Result<List<GetAllLogTypeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllLogTypeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<List<GetAllLogTypeDto>>> Handle(GetAllLogTypeQuery query, CancellationToken cancellationToken)
        {
            var logType = await _unitOfWork.Data<ActivityUser>().Entities
           .Select(g => new GetAllLogTypeDto
           {
              logType = g.LogType

           }).Distinct()
           .ProjectTo<GetAllLogTypeDto>(_mapper.ConfigurationProvider)
           .ToListAsync(cancellationToken);

            return await Result<List<GetAllLogTypeDto>>.SuccessAsync(logType, "Successfully fetch data");
        }

    }
}
