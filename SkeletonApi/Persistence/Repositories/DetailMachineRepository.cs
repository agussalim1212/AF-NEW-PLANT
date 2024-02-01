using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;

namespace SkeletonApi.Persistence.Repositories
{
    public class DetailMachineRepository : IDetailMachineRepository
    {
        private readonly IGenericRepository<Machine> _repository;
        private readonly ApplicationDbContext _dbContext;
        public DetailMachineRepository(ApplicationDbContext dbContext, IGenericRepository<Machine> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetSubjectAirAsync(Guid machineId)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
          .Where(m => machineId == m.MachineId && m.Subject.Vid.Contains("AIR-CONSUMPTION")).ToListAsync();

            string vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();
            var data = new GetAllDetailMachineAirAndElectricConsumptionDto
            {
                Vid = vid,
                MachineName = machineName,
                SubjectName = subjectName
            };
            return data;
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetSubjectPowerAsync(Guid machineId)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
          .Where(m => machineId == m.MachineId && m.Subject.Vid.Contains("POWER-CONSUMPTION")).ToListAsync();

            string vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto
            {
                Vid = vid,
                MachineName = machineName,
                SubjectName = subjectName
            };
            return data;
        }
    }
}
