using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories
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