using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.ManagementUser.Roles.Commands.DeleteRoles;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.DeletePermissions
{
    internal class DeletePermissionsCommandHandler : IRequestHandler<DeletePermissionsRequest, Result<string>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;


        public DeletePermissionsCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        public async Task<Result<string>> Handle(DeletePermissionsRequest request, CancellationToken cancellationToken)
        {
           
            var validateRole = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (validateRole == null)
            {
                return await Result<string>.FailureAsync("Role not found");
            }

            // Remove existing claims for the role
            var existingClaims = await _roleManager.GetClaimsAsync(validateRole);
            foreach (var existingClaim in existingClaims)
            {
                await _roleManager.RemoveClaimAsync(validateRole, existingClaim);
            }

            return await Result<string>.SuccessAsync("Success");
        }
    }
}
