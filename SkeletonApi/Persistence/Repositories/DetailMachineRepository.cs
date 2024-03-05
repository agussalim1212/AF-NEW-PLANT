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
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectMachine;
        private readonly ApplicationDbContext _dbContext;
        public DetailMachineRepository(ApplicationDbContext dbContext, IGenRepository<SubjectHasMachine> repositorySubjectMachine)
        {
            _dbContext = dbContext;
            _repositorySubjectMachine = repositorySubjectMachine;
            
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetSubjectAirAsync(Guid machineId, string vid)
        {
            var machine = await _repositorySubjectMachine.Entities.Include(s => s.Machine).Include(s => s.Subject)
           .Where(m => machineId == m.MachineId && m.Subject.Vid.Contains(vid)).ToListAsync();

            var data = new GetAllDetailMachineAirAndElectricConsumptionDto();

                data = new GetAllDetailMachineAirAndElectricConsumptionDto
                {
                    Vid = machine.Select(p => p.Subject.Vid).FirstOrDefault(),
                    MachineName = machine.Select(o => o.Machine.Name).FirstOrDefault(),
                    SubjectName = machine.Select(p => p.Subject.Subjects).FirstOrDefault()
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
