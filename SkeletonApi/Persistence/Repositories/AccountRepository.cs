using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IGenericRepository<Account> _repository;

        public AccountRepository(IGenericRepository<Account> repository)
        {
            _repository = repository;
        }

        public async Task<bool> ValidateAccount(Account account)
        {
            var x = await _repository.Entities.Where(o => o.Username.ToLower() == account.Username.ToLower()).CountAsync();

            if (x > 0)
            {
                return false;
            }
            return true;
        }

    }
}
