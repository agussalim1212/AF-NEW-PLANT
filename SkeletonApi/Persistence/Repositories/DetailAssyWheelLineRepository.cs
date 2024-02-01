using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;
using System.Globalization;



namespace SkeletonApi.Persistence.Repositories
{
    public class DetailAssyWheelLineRepository : IDetailAssyWheelLineRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public DetailAssyWheelLineRepository(IUnitOfWork unitOfWork, IDapperReadDbConnection dapperReadDbConnection, ApplicationDbContext dbContext)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<GetAllAirConsumptionAssyWheelLineDto> GetAllAirConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines
           .Include(s => s.Machine).Include(s => s.Subject)
           .Where(m => machine_id == m.MachineId && m.Subject.Vid.Contains("AIR-CONSUMPTION")).ToListAsync();

            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllAirConsumptionAssyWheelLineDto();

            var setting = _dbContext.Settings.Where(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = airConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => k.ValueLast),
                            first = g.Select(p => p.ValueFirst).First()
                        }).ToList();

                        if (airConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionAssyWheelLineDto
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
                             new GetAllAirConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = total.Select(val => new AirAssyWheelDto
                                 {
                                     Value = Convert.ToDecimal(val.last) - Convert.ToDecimal(val.last),
                                     Label = val.date_time.AddHours(7).ToString("ddd"),
                                     DateTime = val.date_time,
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
                        var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('month', day_bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', day_bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        //var total = energyConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                        //{
                        //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                        //    first = g.Select(p => p.FirstValue).First()
                        //}).ToList();

                        var groupedQuerys = airConsumption
                          .GroupBy(d => new
                          {
                              d.DayBucket.Month,
                              d.DayBucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_first = g.Sum(d => d.ValueFirst),
                              total_last = g.Sum(d => d.ValueLast),
                          }).ToList();

                        if (airConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionAssyWheelLineDto
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
                             new GetAllAirConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = groupedQuerys.Select(val => new AirAssyWheelDto
                                 {
                                     Value = val.total_last - val.total_first,
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
                        var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('week', day_bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', day_bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

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
                            data = new GetAllAirConsumptionAssyWheelLineDto
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
                            new GetAllAirConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new AirAssyWheelDto
                                {
                                    Value = val.total_last - val.total_first,
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
                        var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('year', day_bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', day_bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY day_bucket DESC", new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        //var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        //{
                        //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                        //    first = g.Select(p => p.FirstValue).First()
                        //}).ToList();

                        var groupedQuerys = airConsumption
                          .GroupBy(d => new
                          {
                              d.DayBucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, 1, 1),
                              total_first = g.Sum(d => d.ValueFirst),
                              total_last = g.Sum(d => d.ValueLast),
                          }).ToList();

                        if (airConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionAssyWheelLineDto
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
                            new GetAllAirConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new AirAssyWheelDto
                                {
                                    Value = val.total_last - val.total_first,
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
                        var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('week', day_bucket) = date_trunc('week', now()) 
                        ORDER BY day_bucket DESC", new { vid = Vid });

                        if (airConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionAssyWheelLineDto
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
                             new GetAllAirConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = airConsumption.Select(val => new AirAssyWheelDto
                                 {
                                     Value = val.ValueLast - val.ValueFirst,
                                     Label = val.DayBucket.AddHours(7).ToString("ddd"),
                                     DateTime = val.DayBucket,
                                 }).OrderByDescending(x => x.DateTime).ToList()

                             };
                        }
                    }
                    break;
            }
            return data;
        }

        public async Task<GetAllEnergyConsumptionAssyWheelLineDto> GetAllEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities
            .Include(s => s.Machine)
            .Include(s => s.Subject).Where(m => machine_id == m.MachineId
             && m.Subject.Vid.Contains("POWER-CONSUMPTION")).ToListAsync();


            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var setting = _dbContext.Settings.Where(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();

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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                        (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                        AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => k.ValueLast),
                            first = g.Sum(p => p.ValueFirst),
                        }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
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
                             new GetAllEnergyConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = total.Select(val => new EnergyAssyDto
                                 {
                                     ValueKwh = val.last - val.first,
                                     ValueCo2 = Math.Round((val.last - val.first) * Convert.ToDecimal(0.87), 2),
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                        (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                        AND date_trunc('month', day_bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', day_bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });


                        var groupedQuerys = energyConsumption
                        .GroupBy(d => new
                        {
                            d.DayBucket.Month,
                            d.DayBucket.Year
                        })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_last = g.Sum(d => d.ValueLast),
                              total_first = g.Sum(d => d.ValueFirst),
                          }).ToList();



                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
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
                             new GetAllEnergyConsumptionAssyWheelLineDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = groupedQuerys.Select(val => new EnergyAssyDto
                                 {
                                     ValueKwh = val.total_last - val.total_first,
                                     ValueCo2 = Math.Round((val.total_last - val.total_first) * Convert.ToDecimal(0.87), 2),
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                        (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                         AND date_trunc('week', day_bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', day_bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY day_bucket DESC",
                         new { vid = Vid, starttime = start.Date, endtime = end.Date });

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
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
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
                            new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new EnergyAssyDto
                                {
                                    ValueKwh = val.total_last - val.total_first,
                                    ValueCo2 = Math.Round((val.total_last - val.total_first) * Convert.ToDecimal(0.87), 2),
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                        (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                        AND date_trunc('year', day_bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', day_bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY day_bucket DESC",
                         new { vid = Vid, starttime = start.Date, endtime = end.Date });


                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              d.DayBucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, 1, 1),
                              total_first = g.Sum(d => d.ValueFirst),
                              total_last = g.Sum(d => d.ValueLast),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionAssyWheelLineDto
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
                            new GetAllEnergyConsumptionAssyWheelLineDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new EnergyAssyDto
                                {
                                    ValueKwh = val.total_last - val.total_first,
                                    ValueCo2 = Math.Round((val.total_last - val.total_first) * Convert.ToDecimal(0.87), 2),
                                    Label = val.date_group.AddHours(7).ToString("yyyy"),
                                    DateTime = val.date_group,
                                }).OrderByDescending(x => x.DateTime).ToList()

                            };
                        }
                    }
                    break;
                default:

                    var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                    (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                    AND date_trunc('week', day_bucket) = date_trunc('week', now()) 
                    ORDER BY day_bucket DESC", new { vid = Vid });

                    var totals = energyConsumptions.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                    {
                        date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        last = g.Sum(k => k.ValueLast),
                        first = g.Select(p => p.ValueFirst).First()
                    }).ToList();

                    if (energyConsumptions.Count() == 0)
                    {
                        data = new GetAllEnergyConsumptionAssyWheelLineDto
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
                         new GetAllEnergyConsumptionAssyWheelLineDto
                         {
                             MachineName = machineName,
                             SubjectName = subjectName,
                             Maximum = setting.Maximum,
                             Medium = setting.Medium,
                             Minimum = setting.Minimum,
                             Data = totals.Select(val => new EnergyAssyDto
                             {
                                 ValueKwh = Convert.ToDecimal(val.last) - Convert.ToDecimal(val.first),
                                 ValueCo2 = Math.Round(((Convert.ToDecimal(val.last) - Convert.ToDecimal(val.first)) * Convert.ToDecimal(0.87)), 2),
                                 Label = val.date_time.AddHours(7).ToString("ddd"),
                                 DateTime = val.date_time,
                             }).OrderByDescending(x => x.DateTime).ToList()
                         };

                    }

                    break;
            }
            return data;
        }

        public async Task<GetAllMachineInformationAssyWheelLineDto> GetAllMachineInformationAsync(Guid machine_id)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
               .Where(m => (machine_id == m.MachineId && m.Subject.Vid.Contains("CYCLE-COUNT"))
               || (machine_id == m.MachineId && m.Subject.Vid.Contains("RUN-TIME"))
               || (machine_id == m.MachineId && m.Subject.Vid.Contains("RIM"))).ToListAsync();


            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllMachineInformationAssyWheelLineDto();

            var categorys = await _dbContext.MachineInformation
             .Where(c => vids.Contains(c.Id))
             .GroupBy(c => c.Id)
             .Select(groups => new
             {
                 Id = groups.Key, // ID dari kelompok
                 LastRunTime = groups.Where(g => g.Id.Contains("RUN-TIME"))
                     .OrderByDescending(g => g.DateTime)
                     .FirstOrDefault(), // Get the last "Run-Time" element
                 LastCycleCount = groups.Where(g => g.Id.Contains("CYCLE-COUNT"))
                     .OrderByDescending(g => g.DateTime)
                     .FirstOrDefault(), // Get the last "Cycle-Count" element
                 LastKalibrasi = groups.Where(g => g.Id.Contains("RIM"))
                     .OrderByDescending(g => g.DateTime)
                     .FirstOrDefault(), // Get the last "rim-calibration" element
             })
             .ToListAsync();


            if (categorys.Count() == 0)
            {
                data = new GetAllMachineInformationAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,

                };

            }
            else
            {


                data = new GetAllMachineInformationAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = DateTime.Now,
                    ValueRunning = categorys.Select(c => Convert.ToDecimal(c.LastRunTime?.Value)).Skip(1).FirstOrDefault(),
                    CycleCount = categorys.Select(x => Convert.ToDecimal(x.LastCycleCount?.Value)).FirstOrDefault(),
                    LastTimeCalibration = categorys.Select(n => n.LastKalibrasi?.Value).Skip(2).FirstOrDefault(),

                };
            }
            return data;
        }
    }
}
    
