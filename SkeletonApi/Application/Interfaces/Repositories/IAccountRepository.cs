using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> ValidateAccount(Account account);
    }
}
