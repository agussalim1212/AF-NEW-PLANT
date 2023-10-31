using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IGenericRepository<Account> _repository;

        public AccountRepository(IGenericRepository<Account> repository)
        {
            _repository = repository;
        }

        public async Task<List<Account>> GetAccountsByClubAsync(Guid clubId)
        {
            return await _repository.Entities.Where(x => x.ClubId == clubId).ToListAsync();
        }
    }
}
