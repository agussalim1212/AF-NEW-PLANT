using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Machine = SkeletonApi.Domain.Entities.Machine;

namespace SkeletonApi.Persistence.Repositories
{
    public class MaintPreventiveRepository : IMaintenancesPreventive
    {
        private readonly IGenericRepository<MaintenacePreventive> _repository;
        private readonly IGenericRepository<Machine> _machineRepository;
        public MaintPreventiveRepository(IGenericRepository<MaintenacePreventive> repository,IGenericRepository<Machine> machineRepository )
        {

            _repository = repository;
            _machineRepository = machineRepository;

        }

        public void DeleteMachines(MaintenacePreventive maintenacePreventive) => _repository.DeleteAsync(maintenacePreventive);

        public async Task<bool> ValidateData(MaintenacePreventive maintenacePreventive)
        {
            var x = await _repository.Entities.Where(o => o.Id == maintenacePreventive.Id).CountAsync();
            if (x > 0)
            {
                return false;
            }
            return true;
        }

        public MaintenacePreventive GetMaintenance(MaintenacePreventive maintenance)
        {
            var category = _machineRepository.Entities
                .Where(o => o.Name.ToLower().Equals(maintenance.Name.ToLower()))
                .Select(o => o.Id).SingleOrDefault();

            var Resp = new MaintenacePreventive()
            {
                Id = maintenance.Id,
                Name = maintenance.Name,
                MachineId = category,
                Plan = maintenance.Plan,
                StartDate = (DateOnly)maintenance.StartDate,         
            };
            return Resp;
        }
    }
}
