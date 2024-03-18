using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AmpereConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Features.ManagementUser.Users.Commands.CreateUser;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories.Filtering
{
    public class MonthRepository : IMonthRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectHasMachine;
        public MonthRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting, IGenRepository<SubjectHasMachine> repositorySubjectHasMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
            _repositorySubjectHasMachine = repositorySubjectHasMachine;
        }

        public async Task<GetAllDetailMachineAirConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirConsumptionDto();
            if (endTime.Value < startTime.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                ORDER BY bucket DESC",
                new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var groupedQuerys = airConsumption
                  .GroupBy(d => new
                  {
                      d.Bucket.Month,
                      d.Bucket.Year
                  })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                      total_first = g.Sum(d => d.ValueFirst),
                      total_last = g.Sum(d => d.ValueLast),
                  }).ToList();

                if (airConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineAirConsumptionDto 
                    { 
                          MachineName = machineName,
                          SubjectName = subjectName 
                    };
                }
                else
                {
                    data =
                     new GetAllDetailMachineAirConsumptionDto
                     {
                         MachineName = machineName,
                         SubjectName = subjectName,
                         //Maximum = setting.Maximum,
                         //Medium = setting.Medium,
                         //Minimum = setting.Minimum,
                         Data = groupedQuerys.Select(val => new DataAir
                         {
                             Value = val.total_last - val.total_first,
                             Label = val.date_group.AddHours(7).ToString("MMM"),
                             DateTime = val.date_group,
                         }).OrderByDescending(x => x.DateTime).ToList()

                     };
                }
                return data;
            }
        }

        public async Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionMonth(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto();

            if (endTime.Value < startTime.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var energyConsumption = await _dapperReadDbConnection.QueryAsync<CurrentConsumptions>
                (@"SELECT * FROM ""current_consumption"" WHERE id = @id
                 AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                 AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                 ORDER BY bucket DESC",
                new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });


                var groupedQuerys = energyConsumption
                .GroupBy(d => new
                {
                    d.Bucket.Month,
                    d.Bucket.Year
                })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                      total_last = g.Sum(d => d.LastValue)
                  }).ToList();

                if (energyConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                    };
                }
                else
                {
                    data =
                     new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                     {
                         MachineName = machineName,
                         SubjectName = subjectName,
                         //Maximum = setting.Maximum,
                         //Medium = setting.Medium,
                         //Minimum = setting.Minimum,
                         Data = groupedQuerys.Select(val => new Data
                         {
                             Value = val.total_last,
                             Label = val.date_group.AddHours(7).ToString("MMM"),
                             DateTime = val.date_group,
                         }).OrderByDescending(x => x.DateTime).ToList()

                     };
                }
                return data;
            }
     
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto();

            if (endTime.Value < startTime.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @id
                AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                ORDER BY bucket DESC",
                new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });


                var groupedQuerys = energyConsumption
                .GroupBy(d => new
                {
                    d.Bucket.Month,
                    d.Bucket.Year
                })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                      total_last = g.Sum(d => d.ValueLast),
                      total_first = g.Sum(d => d.ValueFirst),
                  }).ToList();

                if (energyConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineEnergyConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                    };
                }
                else
                {
                    data =
                     new GetAllDetailMachineEnergyConsumptionDto
                     {
                         MachineName = machineName,
                         SubjectName = subjectName,
                         //Maximum = setting.Maximum,
                         //Medium = setting.Medium,
                         //Minimum = setting.Minimum,
                         Data = groupedQuerys.Select(val => new DataPower
                         {
                             ValueKwh = val.total_last - val.total_first,
                             ValueCo2 = Math.Round((val.total_last - val.total_first) * Convert.ToDecimal(0.87), 2),
                             Label = val.date_group.AddHours(7).ToString("MMM"),
                             DateTime = val.date_group,
                         }).OrderByDescending(x => x.DateTime).ToList()

                     };
                }
                     return data;
            }
        }

        public async Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end)
        {
            var subjectMachine = _repositorySubjectHasMachine.FindByCondition(x => x.Subject.Vid.Contains("POWER-CONSUMPTION")).Include(o => o.Subject).Include(p => p.Machine);
            var subjects = subjectMachine.Select(o => o.Subject.Vid).ToList();
            var setting = _repositorySetting.FindByCondition(o => o.SubjectName == "POWER CONSUMPTION ALL").FirstOrDefault();

            List<GetAllDetailEnergyConsumptionDto> dt = new List<GetAllDetailEnergyConsumptionDto>();
            var data = new GetAllDetailEnergyConsumptionDto();

            if (end.Value < start.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var EnergyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = ANY(@vid)
                AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                ORDER BY bucket DESC",
                new { vid = subjects, starttime = start.Value.Date, endtime = end.Value.Date });

                var Groups = EnergyConsumptions.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month })
                .Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, 1),
                    totalFirst = g.Sum(k => k.ValueFirst),
                    totalLast = g.Sum(k => k.ValueLast)
                }).ToList();

                if (EnergyConsumptions.Count() == 0)
                {
                    dt.Add(data);
                }
                else
                {
                    dt = Groups.Select(o => new GetAllDetailEnergyConsumptionDto
                    {
                        ValueKwh = o.totalLast - o.totalFirst,
                        ValueCo2 = Math.Round((o.totalLast - o.totalFirst) * Convert.ToDecimal(0.87), 2),
                        //Maximum = setting.Maximum,
                        //Medium = setting.Medium,
                        //Minimum = setting.Minimum,
                        Label = o.date_time.ToString("MMM").ToString(),
                        DateTime = o.date_time,

                    }).ToList();
                }
            }
            return dt;
        }
    }
}
