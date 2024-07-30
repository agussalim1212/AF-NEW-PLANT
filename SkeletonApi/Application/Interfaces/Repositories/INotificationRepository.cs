using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Setting>> GetAllSettingAsync();

        Task Creates(IEnumerable<Notifications> mqttrawValues);
        Task Create(Notifications mqttrawValues);
    }
}