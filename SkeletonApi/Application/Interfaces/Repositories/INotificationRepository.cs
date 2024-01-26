using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Setting>> GetAllSettingAsync();
        Task Creates(IEnumerable<Notifications> mqttrawValues);
    }
}
