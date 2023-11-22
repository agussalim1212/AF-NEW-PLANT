using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.CreateRoles
{
    internal class CreateRolesCommandHandler : IRequestHandler<CreateRolesRequest, Result<CreateRolesResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;

        public CreateRolesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<Role> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<Result<CreateRolesResponseDto>> Handle(CreateRolesRequest request, CancellationToken cancellationToken)
        {

            var role = _mapper.Map<Role>(request);
            role.UpdatedAt = DateTime.UtcNow;
            role.CreatedAt = DateTime.UtcNow;

            var validateRole = await _roleManager.FindByNameAsync(role.Name);
            if (validateRole != null)
            {
                return await Result<CreateRolesResponseDto>.FailureAsync("Role already exist.");
            }
            await _roleManager.CreateAsync(role);

            var roleResponse = _mapper.Map<CreateRolesResponseDto>(role);

            return await Result<CreateRolesResponseDto>.SuccessAsync(roleResponse, "Role created.");

        }
    }
}
