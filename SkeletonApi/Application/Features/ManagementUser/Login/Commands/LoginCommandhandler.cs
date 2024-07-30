using MediatR;
using SkeletonApi.Application.Features.ManagementUser;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Login.Commands
{
    internal class LoginCommandHandler : IRequestHandler<UserLoginRequest, Result<TokenDto>>
    {
        private readonly IAuthenticationUserRepository _userRepository;

        public LoginCommandHandler(IAuthenticationUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<TokenDto>> Handle(UserLoginRequest request, CancellationToken cancellationToken)
        {
            //jika username dan password nya benar maka akan membuat token jwt
            if (!await _userRepository.ValidateUser(request))
                return new Result<TokenDto>();
            var tokenDto = await _userRepository.CreateToken(populateExp: true);
            return await Result<TokenDto>.SuccessAsync(tokenDto, "token created.");
        }
    }
}