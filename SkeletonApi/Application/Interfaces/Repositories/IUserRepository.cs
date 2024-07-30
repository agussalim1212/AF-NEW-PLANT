using SkeletonApi.Application.Features.ManagementUser;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ValidateData(User user);

        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);

        Task<TokenDto> CreateToken(bool populateExp);

        // Task<GetPermissionsWithPaginationDto> GetPermissionsWithPaginationDto();
    }
}