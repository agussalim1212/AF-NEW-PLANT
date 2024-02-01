using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Persistence.Repositories.Filtering
{
    public class DayRepository : IDayRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        public DayRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view, string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime)
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
                AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                ORDER BY day_bucket DESC",
                new { id = vid, starttime = startTime.Date, endtime = endTime.Date });

                var total = airConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    last = g.Sum(k => k.ValueLast),
                    first = g.Select(p => p.ValueFirst).First()
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
                         Data = total.Select(val => new DataAir
                         {
                             Value = val.last - val.first,
                             Label = val.date_time.AddHours(7).ToString("ddd"),
                             DateTime = val.date_time,
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
                 AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                 AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                 ORDER BY day_bucket DESC",
                new { id = vid, starttime = startTime.Date, endtime = endTime.Date });

                var total = energyConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    last = g.Sum(k => k.ValueLast),
                    first = g.Sum(p => p.ValueFirst),
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
                    data = new GetAllDetailMachineEnergyConsumptionDto
                     {
                         MachineName = machineName,
                         SubjectName = subjectName,
                         Maximum = setting.Maximum,
                         Medium = setting.Medium,
                         Minimum = setting.Minimum,
                         Data = total.Select(val => new DataPower
                         {
                             ValueKwh = val.last - val.first,
                             ValueCo2 = Math.Round((val.last - val.first) * Convert.ToDecimal(0.87), 2),
                             Label = val.date_time.AddHours(7).ToString("ddd"),
                             DateTime = val.date_time.AddHours(7),
                         }).OrderByDescending(x => x.DateTime).ToList()

                     };
                }
                return data;
            }
        }
    }
}
