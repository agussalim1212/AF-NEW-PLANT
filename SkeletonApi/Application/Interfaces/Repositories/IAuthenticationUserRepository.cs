using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Features.ManagementUser;
using SkeletonApi.Application.Features.ManagementUser.Login.Commands;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IAuthenticationUserRepository
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);

        Task<bool> ValidateUser(UserLoginRequest userForAuth);

        Task<TokenDto> CreateToken(bool populateExp);
    }
}