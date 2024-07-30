using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.CreatePermissions
{
    internal class CreatePermissionsCommandHandler : IRequestHandler<CreatePermissionsRequest, Result<CreatePermissionsResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;

        public CreatePermissionsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<Role> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<Result<CreatePermissionsResponseDto>> Handle(CreatePermissionsRequest request, CancellationToken cancellationToken)
        {
            var permission = _mapper.Map<Permission>(request);

            var validateRole = await _roleManager.FindByNameAsync(request.RoleName);
            if (validateRole == null)
            {
                return await Result<CreatePermissionsResponseDto>.FailureAsync("Role not found.");
            }
            else
            {
                var existingClaims = await _roleManager.GetClaimsAsync(validateRole);
                foreach (var existingClaim in existingClaims)
                {
                    await _roleManager.RemoveClaimAsync(validateRole, existingClaim);
                }
            }

            foreach (var type in request.Claim)
            {
                if (type.ClaimValue == "Edit")
                {
                    permission.ClaimType = type.ClaimValue;
                    permission.ClaimValue = "Permissions.Users.Edit";
                }
                else if (type.ClaimValue == "Delete")
                {
                    permission.ClaimType = type.ClaimValue;
                    permission.ClaimValue = "Permissions.Users.Delete";
                }
                else if (type.ClaimValue == "Create")
                {
                    permission.ClaimType = type.ClaimValue;
                    permission.ClaimValue = "Permissions.Users.Create";
                }
                else
                {
                    permission.ClaimType = type.ClaimValue;
                    permission.ClaimValue = "Permissions.Users.View";
                }

                var pms = new Permission
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RoleId = validateRole.Id,
                    ClaimType = type.ClaimValue,
                    ClaimValue = permission.ClaimValue,
                };

                await _unitOfWork.Data<Permission>().AddAsync(pms);
                await _unitOfWork.Save(cancellationToken);
            }
            var rolePermission = _mapper.Map<CreatePermissionsResponseDto>(permission);

            return await Result<CreatePermissionsResponseDto>.SuccessAsync(rolePermission, "Permissions created.");
        }
    }
}