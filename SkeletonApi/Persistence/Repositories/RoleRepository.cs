using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDataRepository<Role> _repository;
        private readonly RoleManager<Role> _roleManager;

        public async Task<bool> ValidateData(Role role)
        {
            var x = await _roleManager.FindByNameAsync(role.Name);
            if (x == null)
            {
                return true;
            }
            return false;

            //var x = await _repository.Entities.Where(o => o.Name.ToLower() == user.Name.ToLower()).CountAsync();

            //if (x > 0)
            //{
            //    return false;
            //}
            //return true;
        }
    }
}