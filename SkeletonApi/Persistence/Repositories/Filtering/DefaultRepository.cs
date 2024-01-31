using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Persistence.Repositories.Filtering
{
    public class DefaultRepository : IDefaultRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        public DefaultRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirAndElectricConsumptionDto();
     
                var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @id
                AND date_trunc('week', day_bucket) = date_trunc('week', now()) 
                ORDER BY day_bucket DESC", new { id = vid });

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
                         Data = airConsumption.Select(val => new DataAir
                         {
                             Value = val.ValueLast - val.ValueFirst,
                             Label = val.DayBucket.AddHours(7).ToString("ddd"),
                             DateTime = val.DayBucket,
                         }).OrderByDescending(x => x.DateTime).ToList()

                     };
                }
            return data;
            }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto();
            var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                    (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @id
                    AND date_trunc('week', day_bucket) = date_trunc('week', now()) 
                    ORDER BY day_bucket DESC", new { id = vid });

            var totals = energyConsumptions.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
            {
                date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                last = g.Sum(k => k.ValueLast),
                first = g.Select(p => p.ValueFirst).First()
            }).ToList();

            if (energyConsumptions.Count() == 0)
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
                     Data = totals.Select(val => new DataPower
                     {
                         ValueKwh = val.last - val.first,
                         ValueCo2 = Math.Round((val.last - val.first) * Convert.ToDecimal(0.87), 2),
                         Label = val.date_time.AddHours(7).ToString("ddd"),
                         DateTime = val.date_time,
                     }).OrderByDescending(x => x.DateTime).ToList()
                 };
            }
            return data;
        }
    }
}
