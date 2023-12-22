using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.UpdatePermissions
{
     internal class UpdatePermissionsCommand : IRequestHandler<UpdatePermissionsRequest, Result<Permission>>
     {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;

        public UpdatePermissionsCommand(IUnitOfWork unitOfWork, RoleManager<Role> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        public async Task<Result<Permission>> Handle(UpdatePermissionsRequest request, CancellationToken cancellationToken)
        {
          
            var validateRole = await _roleManager.FindByIdAsync(request.Id);
            if (validateRole != null)
            {
                // Remove existing claims for the role
                var existingClaims = await _roleManager.GetClaimsAsync(validateRole);
                foreach (var existingClaim in existingClaims)
                {
                    await _roleManager.RemoveClaimAsync(validateRole, existingClaim);
                }
            }

            if(request.Claim.Count == 0)
            {
                return await Result<Permission>.FailureAsync("Permissions cannot be null.");
            }

            foreach (var type in request.Claim)
            {
               
                var pms = new Permission
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RoleId = validateRole.Id,
                    ClaimType = type.ClaimValue,
                    ClaimValue = type.ClaimValue,
                };

                if (type.ClaimValue == "Edit")
                {
                    pms.ClaimType = type.ClaimValue;
                    pms.ClaimValue = "Permissions.Users.Edit";
                }
                else if (type.ClaimValue == "Delete")
                {
                    pms.ClaimType = type.ClaimValue;
                    pms.ClaimValue = "Permissions.Users.Delete";
                }
                else if (type.ClaimValue == "Create")
                {
                    pms.ClaimType = type.ClaimValue;
                    pms.ClaimValue = "Permissions.Users.Create";
                }
                else
                {
                    pms.ClaimType = type.ClaimValue;
                    pms.ClaimValue = "Permissions.Users.View";
                }

                await _unitOfWork.Data<Permission>().AddAsync(pms);
                await _unitOfWork.Save(cancellationToken);
            }

            return await Result<Permission>.SuccessAsync("Permissions Update.");

        }
    }

}
