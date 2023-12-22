using SkeletonApi.Application.Features.ManagementUser.Permissions.Queries.GetRoleWithPagination;
using SkeletonApi.Application.Features.Users;
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
