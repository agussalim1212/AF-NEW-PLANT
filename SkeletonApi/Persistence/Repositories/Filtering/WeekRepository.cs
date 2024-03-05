using DocumentFormat.OpenXml.Vml;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Domain.Entities;
using System.Globalization;

namespace SkeletonApi.Persistence.Repositories.Filtering
{
    public class WeekRepository : IWeekRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectHasMachine;
        public WeekRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting, IGenRepository<SubjectHasMachine> repositorySubjectHasMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
            _repositorySubjectHasMachine = repositorySubjectHasMachine;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view,string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirAndElectricConsumptionDto();
            if (endTime.Date < startTime.Date)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                ($@"SELECT * FROM {view} WHERE id = @id
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY bucket DESC",
                new { id = vid, starttime = startTime.Date, endtime = endTime.Date });

                //var total = energyConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                //{
                //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                //    first = g.Select(p => p.FirstValue).First()
                //}).ToList();

                var groupedQuerys = airConsumption
                  .GroupBy(d => new
                  {
                      //o.DateTime.Year,
                      //o.DateTime.Month,
                      WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                  })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                      total_first = g.Sum(d => d.ValueFirst),
                      total_last = g.Sum(d => d.ValueLast),
                  }).ToList();

                if (airConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineAirAndElectricConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName
                    };
                }
                else
                {

                    data =
                    new GetAllDetailMachineAirAndElectricConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        //Maximum = setting.Maximum,
                        //Medium = setting.Medium,
                        //Minimum = setting.Minimum,
                        Data = groupedQuerys.Select(val => new DataAir
                        {
                            Value = val.total_last - val.total_first,
                            Label = "Week " + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(val.date_group, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(),
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
                AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                ORDER BY bucket DESC",
                 new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var groupedQuerys = energyConsumption
                .GroupBy(d => new
                {
                    WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
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
                            Label = "Week " + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(val.date_group, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(),
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
                AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                ORDER BY bucket DESC",
                new { vid = subjects, starttime = start.Value.Date, endtime = end.Value.Date });

                var Groups = EnergyConsumptions.GroupBy(p => new 
                {
                    WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(p.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                })
                .Select(g => new
                {
                    date_time = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
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
                        Label = "Week " + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.date_time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(),
                        DateTime = o.date_time,

                    }).ToList();
                }
            }
            return dt;
        }
    }
}
