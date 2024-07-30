using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ActivityUsers.Queries.GetAllUsername
{
    public record GetAllUsernameQuery : IRequest<Result<List<GetAllUsernameDto>>>;

    internal class GetAllUsernameQueryHandler : IRequestHandler<GetAllUsernameQuery, Result<List<GetAllUsernameDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUsernameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllUsernameDto>>> Handle(GetAllUsernameQuery query, CancellationToken cancellationToken)
        {
            var username = await _unitOfWork.Data<ActivityUser>().Entities
           .Select(g => new GetAllUsernameDto
           {
               UserName = g.UserName
           }).Distinct()
           .ProjectTo<GetAllUsernameDto>(_mapper.ConfigurationProvider)
           .ToListAsync(cancellationToken);

            return await Result<List<GetAllUsernameDto>>.SuccessAsync(username, "Successfully fetch data");
        }
    }
}