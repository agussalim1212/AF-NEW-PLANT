using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface ISettingRepository
    {
        Task<bool> ValidateSetting(Setting setting);
    }
}