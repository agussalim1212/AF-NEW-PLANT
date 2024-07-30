using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> ValidateData(Role role);
    }
}