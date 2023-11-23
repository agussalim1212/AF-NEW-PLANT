using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IMaintenancesPreventive
    {
        Task<bool> ValidateData(MaintenacePreventive maintenacePreventive);

        void DeleteMachines(MaintenacePreventive maintenacePreventive);
    }
}
