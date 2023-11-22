using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Security.Claims;

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
          
            var validateRole = await _roleManager.FindByNameAsync(request.RoleName);
            if (validateRole == null) 
            {
                return await Result<CreatePermissionsResponseDto>.FailureAsync("Role not found.");
            }

            foreach (var type in request.Claim)
            {
                

                if (type.ClaimValue == "Edit")
                {
                    Claim claims = new Claim("Edit", "Permissions.Users.Edit");
                    await _roleManager.AddClaimAsync(validateRole, claims);
                }
                else if (type.ClaimValue == "Delete")
                {
                    Claim claims = new Claim("Delete", "Permissions.Users.Delete");
                    await _roleManager.AddClaimAsync(validateRole, claims);
                }
                else if (type.ClaimValue == "Create")
                {
                    Claim claims = new Claim("Create", "Permissions.Users.Create");
                    await _roleManager.AddClaimAsync(validateRole, claims);
                }
                else
                {
                    Claim claims = new Claim("View", "Permissions.Users.View");
                    await _roleManager.AddClaimAsync(validateRole, claims);
                }  
            }

            return await Result<CreatePermissionsResponseDto>.SuccessAsync("Permissions created.");

        }
    }
}
