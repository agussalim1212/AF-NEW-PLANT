using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.DeleteRoles
{
    internal class DeleteRolesCommandHandler : IRequestHandler<DeleteRolesRequest, Result<string>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;

        public DeleteRolesCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        public async Task<Result<string>> Handle(DeleteRolesRequest request, CancellationToken cancellationToken)
        {
            var validateRole = await _roleManager.FindByIdAsync(request.Id);
            if (validateRole == null)
            {
                return await Result<string>.FailureAsync("Role not found");
            }
            validateRole.DeletedAt = DateTime.UtcNow;
            await _roleManager.UpdateAsync(validateRole);

            return await Result<string>.SuccessAsync("Success");
        }
    }
}