using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Users.Login.Commands
{
    internal class LoginCommandHandler : IRequestHandler<UserLoginRequest, Result<TokenDto>>
    {
        private readonly IAuthenticationUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public LoginCommandHandler(UserManager<User> userManager, IAuthenticationUserRepository userRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<Result<TokenDto>> Handle(UserLoginRequest request, CancellationToken cancellationToken)
        {
            if (!await _userRepository.ValidateUser(request))
                return new Result<TokenDto>();
            var tokenDto = await _userRepository.CreateToken(populateExp: true);
            return await Result<TokenDto>.SuccessAsync(tokenDto,"token created.");

        }

    }
}
