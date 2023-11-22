using SkeletonApi.Application.Features.Users;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ValidateData(User user);
    }
}
