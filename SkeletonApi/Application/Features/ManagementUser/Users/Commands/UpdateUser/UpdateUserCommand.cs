using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;



namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.UpdateUser
{
    internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserRequest, Result<User>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }
        private async Task<List<string>> GetUserRoles(User user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));

        }
        public async Task<Result<User>> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var validateUser = await _userManager.FindByIdAsync(request.Id);
            if (validateUser == null)
            {
                return await Result<User>.FailureAsync("User not found");
            }
            validateUser.UpdatedAt = DateTime.UtcNow;
            validateUser.UserName = request.UserName;
            validateUser.Email = request.Email;

            var result = await _userManager.UpdateAsync(validateUser);
            if (result.Succeeded)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(validateUser);
                validateUser.Roles = request.Roles;
                await _userManager.ResetPasswordAsync(validateUser, token, request.PasswordHash);
                await _userManager.RemoveFromRolesAsync(validateUser, await GetUserRoles(validateUser));
                await _userManager.AddToRolesAsync(validateUser, request.Roles);
            }

            return await Result<User>.SuccessAsync("Success");
        }
    }
}
