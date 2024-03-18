using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.DTOs.DetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;


namespace SkeletonApi.Persistence.Repositories
{
    public class DetailMachineRepository : IDetailMachineRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectMachine;
        private readonly ApplicationDbContext _dbContext;
        public DetailMachineRepository(IDapperReadDbConnection dapperReadDbConnection, ApplicationDbContext dbContext, IGenRepository<SubjectHasMachine> repositorySubjectMachine)
        {
            _dbContext = dbContext;
            _repositorySubjectMachine = repositorySubjectMachine;
            _dapperReadDbConnection = dapperReadDbConnection;
            
        }

        public async Task<GetAllMachineInformationDto> GetAllMachineInformationAsync(Guid machine_id, string vidRunning, string vidReminder, string machineName)
        {
            var data = new GetAllMachineInformationDto();

            var runningAndReminder = await _dapperReadDbConnection.QueryAsync<MachineInformationConsumption>
                       (@"select id,value ,date_time  from ""MachineInformation"" mi where id = @vidRun OR id = @vidRemind order by date_time desc limit 2",
                       new { vidRun = vidRunning, vidRemind = vidReminder });

            if (runningAndReminder.Count() == 0)
            {
                data = new GetAllMachineInformationDto
                {
                    MachineName = machineName,
                };

            }
            else
            {

                data = new GetAllMachineInformationDto
                {
                    MachineName = machineName,
                    LastTimeCalibration = runningAndReminder.Select(p => p.Value).FirstOrDefault(),
                    ValueRunning = runningAndReminder.Select(p => p.Value).Skip(1).FirstOrDefault(),

                };
            }
            return data;
        }

        public async Task<GetVidSubjectDto> GetSubjectAsync(Guid machineId, string vid)
        {
            var machine = await _repositorySubjectMachine.Entities.Include(s => s.Machine).Include(s => s.Subject)
           .Where(m => machineId == m.MachineId && m.Subject.Vid.Contains(vid)).ToListAsync();

            var data = new GetVidSubjectDto();

                data = new GetVidSubjectDto
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
