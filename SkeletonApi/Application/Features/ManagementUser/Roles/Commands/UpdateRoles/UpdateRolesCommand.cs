using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.UpdateRoles
{
    internal class UpdateRolesCommand : IRequestHandler<UpdateRolesRequest, Result<Role>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;

        public UpdateRolesCommand(IMapper mapper, IUnitOfWork unitOfWork, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        public async Task<Result<Role>> Handle(UpdateRolesRequest request, CancellationToken cancellationToken)
        {
            var validateRole = await _roleManager.FindByIdAsync(request.Id);
            if (validateRole == null)
            {
                return await Result<Role>.FailureAsync("Role not found");
            }
            validateRole.UpdatedAt = DateTime.UtcNow;
            validateRole.Name = request.Name;
            await _roleManager.UpdateAsync(validateRole);

            return await Result<Role>.SuccessAsync("Success");
        }
    }
}