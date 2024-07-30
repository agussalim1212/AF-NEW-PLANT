using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.DeleteUser
{
    internal class DeleteUserCommandHandle : IRequestHandler<DeleteUserRequest, Result<string>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public DeleteUserCommandHandle(IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var validateUser = await _userManager.FindByIdAsync(request.Id);
            if (validateUser == null)
            {
                return await Result<string>.FailureAsync("User not found");
            }
            validateUser.DeletedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(validateUser);

            return await Result<string>.SuccessAsync("Success");
        }
    }
}