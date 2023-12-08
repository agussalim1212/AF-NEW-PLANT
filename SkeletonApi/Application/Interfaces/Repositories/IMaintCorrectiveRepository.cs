using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IMaintCorrectiveRepository
    {
        Task<bool> ValidateData(MaintCorrective maintenanceCorrective);

        void DeleteMaintCorrective(MaintCorrective maintenanceCorrective);
    }
}
