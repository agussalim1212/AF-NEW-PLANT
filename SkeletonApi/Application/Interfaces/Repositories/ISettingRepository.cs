using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Persistence.Repositories
{
    public interface ISettingRepository
    {
        Task<bool> ValidateSetting(Setting setting);
    }
}
