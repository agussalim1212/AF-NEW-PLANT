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
        public WeekRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime)
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
                (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @id
                        AND date_trunc('week', day_bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', day_bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY day_bucket DESC",
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
                      WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.DayBucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
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
                        SubjectName = subjectName,
                        Maximum = setting.Maximum,
                        Medium = setting.Medium,
                        Minimum = setting.Minimum,
                    };
                }
                else
                {

                    data =
                    new GetAllDetailMachineAirAndElectricConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        Maximum = setting.Maximum,
                        Medium = setting.Medium,
                        Minimum = setting.Minimum,
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

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto();
            if (endTime.Date < startTime.Date)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @id
                         AND date_trunc('week', day_bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', day_bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY day_bucket DESC",
                 new { id = vid, starttime = startTime.Date, endtime = endTime.Date });

                var groupedQuerys = energyConsumption
                .GroupBy(d => new
                {

                    WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.DayBucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
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
                        Maximum = setting.Maximum,
                        Medium = setting.Medium,
                        Minimum = setting.Minimum,
                    };
                }
                else
                {

                    data =
                    new GetAllDetailMachineEnergyConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        Maximum = setting.Maximum,
                        Medium = setting.Medium,
                        Minimum = setting.Minimum,
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
    }
}
