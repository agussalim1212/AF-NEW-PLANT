using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Globalization;



namespace SkeletonApi.Persistence.Repositories
{
    public class DetailAssyWheelLineRepository : IDetailAssyWheelLineRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IUnitOfWork _unitOfWork;

        public DetailAssyWheelLineRepository(IUnitOfWork unitOfWork, IDapperReadDbConnection dapperReadDbConnection)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetAllEnergyConsumptionAssyWheelLineDto> GetAllEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => machine_id == m.MachineId
             && m.Subject.Vid.Contains("POWER-CONSUMPTION")).ToListAsync();
            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();


            var data = new GetAllEnergyConsumptionAssyWheelLineDto();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER bucket ASC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                            first = g.Select(p => p.FirstValue).First()
                        }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {
                            data =
                             new GetAllEnergyConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = total.Select(val => new EnergyAssyDto
                                 {
                                     ValueKwh = Convert.ToDecimal(val.last) - Convert.ToDecimal(val.first),
                                     ValueCo2 = Math.Round((Convert.ToDecimal(val.last) - Convert.ToDecimal(val.first) * Convert.ToDecimal(0.87)), 2),
                                     Label = val.date_time.AddHours(7).ToString("ddd"),
                                     DateTime = val.date_time.AddHours(7),
                                 }).OrderByDescending(x => x.DateTime).ToList()

                             };
                        }
                    }
                    break;
                case "month":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY bucket ASC", new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                            first = g.Select(p => p.FirstValue).First()
                        }).ToList();

                        var groupedQuerys = total
                          .GroupBy(d => new
                          {
                              d.date_time.Month,
                              d.date_time.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_first = g.Sum(d => Convert.ToDecimal(d.first)),
                              total_last = g.Sum(d => Convert.ToDecimal(d.last)),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {
                            data =
                             new GetAllEnergyConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = groupedQuerys.Select(val => new EnergyAssyDto
                                 {
                                     ValueKwh = Convert.ToDecimal(val.total_last) - Convert.ToDecimal(val.total_first),
                                     ValueCo2 = Math.Round((Convert.ToDecimal(val.total_last) - Convert.ToDecimal(val.total_first) * Convert.ToDecimal(0.87)), 2),
                                     Label = val.date_group.AddHours(7).ToString("MMM"),
                                     DateTime = val.date_group,
                                 }).OrderByDescending(x => x.DateTime).ToList()

                             };
                        }
                    }
                    break;
                case "week":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                         AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY bucket ASC", new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                            first = g.Select(p => p.FirstValue).First()
                        }).ToList();

                        var groupedQuerys = total
                          .GroupBy(d => new
                          {
                              //o.DateTime.Year,
                              //o.DateTime.Month,
                              WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.date_time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                              total_first = g.Sum(d => Convert.ToDecimal(d.first)),
                              total_last = g.Sum(d => Convert.ToDecimal(d.last)),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Data = groupedQuerys.Select(val => new EnergyAssyDto
                                {
                                    ValueKwh = Convert.ToDecimal(val.total_last) - Convert.ToDecimal(val.total_first),
                                    ValueCo2 = Math.Round((Convert.ToDecimal(val.total_last) - Convert.ToDecimal(val.total_first) * Convert.ToDecimal(0.87)), 2),
                                    Label = "Week " + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(val.date_group, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(),
                                    DateTime = val.date_group,
                                }).OrderByDescending(x => x.DateTime).ToList()

                            };
                        }
                    }
                    break;
                case "year":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY bucket ASC", new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                            first = g.Select(p => p.FirstValue).First()
                        }).ToList();

                        var groupedQuerys = total
                          .GroupBy(d => new
                          {
                              d.date_time.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, 1, 1),
                              total_first = g.Sum(d => Convert.ToDecimal(d.first)),
                              total_last = g.Sum(d => Convert.ToDecimal(d.last)),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Data = groupedQuerys.Select(val => new EnergyAssyDto
                                {
                                    ValueKwh = Convert.ToDecimal(val.total_last) - Convert.ToDecimal(val.total_first),
                                    ValueCo2 = Math.Round((Convert.ToDecimal(val.total_last) - Convert.ToDecimal(val.total_first) * Convert.ToDecimal(0.87)), 2),
                                    Label = val.date_group.AddHours(7).ToString("yyy"),
                                    DateTime = val.date_group,
                                }).OrderByDescending(x => x.DateTime).ToList()

                            };
                        }
                    }
                    break;
                default:
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                         AND date_trunc('week', bucket) = date_trunc('week', now()) 
                         ORDER BY id DESC, bucket DESC", new { vid = Vid });

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                             new GetAllEnergyConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = energyConsumption.Select(val => new EnergyAssyDto
                                 {
                                     ValueKwh = Convert.ToDecimal(val.FirstValue) - Convert.ToDecimal(val.LastValue),
                                     ValueCo2 = Math.Round((Convert.ToDecimal(val.FirstValue) - Convert.ToDecimal(val.LastValue) * Convert.ToDecimal(0.87)), 2),
                                     Label = val.Bucket.AddHours(7).ToString("ddd"),
                                     DateTime = val.Bucket,
                                 }).OrderByDescending(x => x.DateTime).ToList()

                             };
                        }
                    }
                    break;
            }
            return data;
        }

    }
}
    
