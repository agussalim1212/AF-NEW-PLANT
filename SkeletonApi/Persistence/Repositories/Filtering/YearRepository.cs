using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.Consumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories.Filtering
{
    public class YearRepository : IYearRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectHasMachine;

        public YearRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting, IGenRepository<SubjectHasMachine> repositorySubjectHasMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
            _repositorySubjectHasMachine = repositorySubjectHasMachine;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionYear(string view, string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime)
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
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY bucket DESC", new { id = vid, starttime = startTime.Date, endtime = endTime.Date });

                //var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                //{
                //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                //    first = g.Select(p => p.FirstValue).First()
                //}).ToList();

                var groupedQuerys = airConsumption
                  .GroupBy(d => new
                  {
                      d.Bucket.Year
                  })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.Year, 1, 1),
                      total_first = g.Sum(d => d.ValueFirst),
                      total_last = g.Sum(d => d.ValueLast),
                  }).ToList();

                if (airConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineAirAndElectricConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
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
                            Label = val.date_group.ToString("yyy"),
                            DateTime = val.date_group,
                        }).OrderByDescending(x => x.DateTime).ToList()
                    };
                }
                return data;
            }
        }

        public async Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionYear(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
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
                 ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                ORDER BY bucket DESC",
                 new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var groupedQuerys = energyConsumption
                .GroupBy(d => new
                {
                    d.Bucket.Year
                })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.Year, 1, 1),
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
                            Label = val.date_group.ToString("yyy"),
                            DateTime = val.date_group,
                        }).OrderByDescending(x => x.DateTime).ToList()
                    };
                }
                return data;
            }
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionYear(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
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
                AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                ORDER BY bucket DESC",
                 new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var groupedQuerys = energyConsumption
                .GroupBy(d => new
                {
                    d.Bucket.Year
                })
                  .Select(g => new
                  {
                      date_group = new DateTime(g.Key.Year, 1, 1),
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
                            Label = val.date_group.ToString("yyy"),
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
                AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                ORDER BY bucket DESC",
                new { vid = subjects, starttime = start.Value.Date, endtime = end.Value.Date });

                var Groups = EnergyConsumptions.GroupBy(p => new { p.Bucket.Year })
                .Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, 1, 1),
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
                        Label = o.date_time.ToString("yyy").ToString(),
                        DateTime = o.date_time,
                    }).ToList();
                }
            }
            return dt;
        }

        public async Task<GetAllTotalProductionDto> GetAllTotalProductionYear(Guid machineId, string vidOK, string vidNG, string machineName, DateTime? start, DateTime? end)
        {
            var data = new GetAllTotalProductionDto();

            if (end.Value.Date < start.Value.Date)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                (@"SELECT * FROM ""production_consumption"" WHERE id = @vidok OR id = @vidng
                AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)",
                new { vidok = vidOK, vidng = vidNG, starttime = start.Value.Date, endtime = end.Value.Date });

                var groupedQuerys = consumptionBucket
                .GroupBy(d => new
                {
                    d.Bucket.Year,
                })
                .Select(g => new
                {
                    date_group = new DateTime(g.Key.Year, 1, 1),
                    total_ok = g.Where(p => p.Id.Contains(vidOK)).Sum(o => Convert.ToDecimal(o.LastValue)),
                    total_ng = g.Where(p => p.Id.Contains(vidNG)).Sum(o => Convert.ToDecimal(o.LastValue)),
                }).ToList();

                decimal TotalOk = groupedQuerys.Select(o => o.total_ok).FirstOrDefault();
                decimal TotalNg = groupedQuerys.Select(p => p.total_ng).FirstOrDefault();

                if (consumptionBucket.Count() == 0)
                {
                    data = new GetAllTotalProductionDto
                    {
                        MachineName = machineName
                    };
                }
                else
                {
                    data =
                    new GetAllTotalProductionDto
                    {
                        MachineName = machineName,
                        ValueOkTotal = TotalOk,
                        ValueNgTotal = TotalNg,
                        ValueOKPresentase = Math.Round((TotalOk / (TotalOk + TotalNg)) * 100, 2),
                        ValueNgPresentase = Math.Round((TotalNg / (TotalNg + TotalOk)) * 100, 2),
                    };
                }
                return data;
            }
        }
    }
}