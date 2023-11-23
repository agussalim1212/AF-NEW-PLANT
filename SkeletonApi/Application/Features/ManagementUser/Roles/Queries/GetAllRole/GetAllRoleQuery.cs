using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetAllRole
{
     public record GetAllRoleQuery : IRequest<Result<List<GetAllRoleDto>>>;

    internal class GetAllRoleQueryHandler : IRequestHandler<GetAllRoleQuery, Result<List<GetAllRoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllRoleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<List<GetAllRoleDto>>> Handle(GetAllRoleQuery query, CancellationToken cancellationToken)
        {
            var role = await _unitOfWork.Data<Role>().FindByCondition(h => h.DeletedAt == null)
           .Select(g => new GetAllRoleDto
            {
                Id = g.Id,
                Name = g.Name,
               
            })
            .ProjectTo<GetAllRoleDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

            return await Result<List<GetAllRoleDto>>.SuccessAsync(role, "Successfully fetch data");
        }

    }
}
