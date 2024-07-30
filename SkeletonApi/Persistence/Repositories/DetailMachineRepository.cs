using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.DTOs.Consumption;
using SkeletonApi.Application.DTOs.DetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityDataBarcodeWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
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
            DateTime today = DateTime.Now.Date.AddHours(7);
            var running = await _dapperReadDbConnection.QueryAsync<MachineInformationConsumption>
                       (@"select id,value ,date_time  from ""MachineInformation"" mi where id = @vidRun order by date_time desc limit 1",
                       new { vidRun = vidRunning });

            var reminder = await _dapperReadDbConnection.QueryAsync<MachineInformationConsumption>
                       (@"select id,value ,date_time  from ""MachineInformation"" mi where id = @vidRemind order by date_time desc limit 1",
                       new { vidRemind = vidReminder });

            if (running.Count() == 0 && reminder.Count() == 0)
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
                    ValueRunning = running.Select(p => p.Value).FirstOrDefault(),
                    LastTimeCalibration = reminder.Select(p => p.Value).FirstOrDefault(),
                };
            }
            return data;
        }

        public async Task<List<GetListQualityBarcodeDto>> GetListQualitYBarcode(string vid)
        {
            var data = await _dapperReadDbConnection.QueryAsync<ListQualityBarcodeDto>(@$"select * from ""ListQualityBarcodes"" lqb where lqb.id = '{vid}' and lqb.data_barcode != 'NULL' and date(lqb.date_time) = current_date order by lqb.date_time desc");
            List<GetListQualityBarcodeDto> listQuality = new List<GetListQualityBarcodeDto>();
            var dataBarcode = new GetListQualityBarcodeDto();
            if (data.Count() == 0)
            {
                dataBarcode.DataBarcode = null;
                dataBarcode.Status = null;
                dataBarcode.FotoDataNg = null;
                dataBarcode.DateTime = null;
            }
            else
            {
                listQuality = data.Select(p => new GetListQualityBarcodeDto
                {
                    DataBarcode = p.DataBarcode,
                    Status = p.Status,
                    FotoDataNg = p.FotoDataNg,
                    DateTime = p.DateTime.AddHours(7)
                }).ToList();
            }

            return listQuality;
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