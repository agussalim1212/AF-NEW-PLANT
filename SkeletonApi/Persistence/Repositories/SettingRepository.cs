using Microsoft.EntityFrameworkCore;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Repositories;


namespace SkeletonApi.Application.Interfaces.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly IGenericRepository<Setting> _repository;

        public async Task<bool> ValidateSetting(Setting setting)
        {
            var x = await _repository.Entities.Where(o => o.MachineName.ToLower() == setting.MachineName.ToLower()).CountAsync();
            if (x > 0)
            {
                return false;

            }
            return true;
        }
    }
}
