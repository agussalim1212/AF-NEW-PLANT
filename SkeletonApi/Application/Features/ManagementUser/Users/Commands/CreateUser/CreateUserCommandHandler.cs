using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Exceptions;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.CreateUser
{
    internal class CreateUserCommandHandler : IRequestHandler<CreateUserRequest, Result<CreateUserResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;



        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<Result<CreateUserResponseDto>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {

            var user = _mapper.Map<User>(request);
            user.UpdatedAt = DateTime.UtcNow;
            user.CreatedAt = DateTime.UtcNow;
            var result = await _userManager.CreateAsync(user, request.Password);

            foreach (var role in request.Roles)
            {
                var validateRole = await _roleManager.FindByNameAsync(role);
                if (validateRole == null)
                {
                    return await Result<CreateUserResponseDto>.FailureAsync("Role not found.");
                }
            }


            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, request.Roles);
            }
            else
            {
                throw new FailedAuthenticationException($": Failed to create user, ${result.Errors}.");
            }
          


            var userResponse = _mapper.Map<CreateUserResponseDto>(user);

            return await Result<CreateUserResponseDto>.SuccessAsync(userResponse, "User created.");

        }
    }
}
