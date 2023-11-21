using SkeletonApi.Application.Features.Users;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<CreateUserResponseDto> CreateUserAsync(UserForRegistrationDto userDto);
    }
}
