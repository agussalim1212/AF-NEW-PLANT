using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.FrequencyInverter;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrake;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRace;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;
using System.Globalization;



namespace SkeletonApi.Persistence.Repositories
{
    public class DetailAssyUnitRepository : IDetailAssyUnitRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly ApplicationDbContext _dbContext;
        public DetailAssyUnitRepository(IUnitOfWork unitOfWork, IDapperReadDbConnection dapperReadDbConnection, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dapperReadDbConnection = dapperReadDbConnection;
            _dbContext = dbContext;
        }

        public async Task<GetAllAirConsumptionDto> GetAllAirConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines
            .Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => machine_id == m.MachineId && m.Subject.Vid.Contains("AIR-CONSUMPTION")).ToListAsync();

            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllAirConsumptionDto();

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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                            first = g.Select(p => p.FirstValue).First()
                        }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionDto
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
                             new GetAllAirConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = total.Select(val => new AirDto
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
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

                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              d.DayBucket.Month,
                              d.DayBucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_first = g.Sum(d => d.FirstValue),
                              total_last = g.Sum(d => d.LastValue),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionDto
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
                             new GetAllAirConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = groupedQuerys.Select(val => new AirDto
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
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

                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              //o.DateTime.Year,
                              //o.DateTime.Month,
                              WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.DayBucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                              total_first = g.Sum(d => d.FirstValue),
                              total_last = g.Sum(d => d.LastValue),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionDto
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
                            new GetAllAirConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new AirDto
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('year', day_bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', day_bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY day_bucket DESC", new {vid = Vid, starttime = start.Date, endtime = end.Date });

                        //var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        //{
                        //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                        //    first = g.Select(p => p.FirstValue).First()
                        //}).ToList();

                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              d.DayBucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, 1, 1),
                              total_first = g.Sum(d => d.FirstValue),
                              total_last = g.Sum(d => d.LastValue),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionDto
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
                            new GetAllAirConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new AirDto
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                        (@"SELECT * FROM ""air_consumption_setting"" WHERE id = @vid
                        AND date_trunc('week', day_bucket) = date_trunc('week', now()) 
                        ORDER BY day_bucket DESC", new { vid = Vid });

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllAirConsumptionDto
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
                             new GetAllAirConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = energyConsumption.Select(val => new AirDto
                                 {
                                     Value = val.LastValue - val.FirstValue,
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
        public async Task<GetAllElectricGeneratorConsumptionDto> GetAllElectricGeneratorConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => machine_id == m.MachineId
             && m.Subject.Vid.Contains("ELECT_GNTR")).ToListAsync();
            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllElectricGeneratorConsumptionDto();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<ElectricConsumptionDetail>
                        (@"SELECT * FROM ""electric_consumption"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC"
                        , new { vid = Vid, starttime = start.Date, endtime = end.Date });


                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {
                            data =
                             new GetAllElectricGeneratorConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = energyConsumption.Select(val => new ElectricDto
                                 {
                                     Value = Convert.ToDecimal(val.FirstValue) - Convert.ToDecimal(val.LastValue),
                                     Label = val.Bucket.AddHours(7).ToString("ddd"),
                                     DateTime = val.Bucket,
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<ElectricConsumptionDetail>
                        (@"SELECT * FROM ""electric_consumption"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              d.Bucket.Month,
                              d.Bucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_first = g.Sum(d => Convert.ToDecimal(d.FirstValue)),
                              total_last = g.Sum(d => Convert.ToDecimal(d.LastValue)),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {
                            data =
                             new GetAllElectricGeneratorConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = groupedQuerys.Select(val => new ElectricDto
                                 {
                                     Value = val.total_first - val.total_last,
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<ElectricConsumptionDetail>
                        (@"SELECT * FROM ""electric_consumption"" WHERE id = @vid
                       AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                       AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                       ORDER BY id DESC, bucket DESC"
                        , new { vid = Vid, starttime = start.Date, endtime = end.Date });
                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              //o.DateTime.Year,
                              //o.DateTime.Month,
                              WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                              total_first = g.Sum(d => Convert.ToDecimal(d.FirstValue)),
                              total_last = g.Sum(d => Convert.ToDecimal(d.LastValue)),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Data = groupedQuerys.Select(val => new ElectricDto
                                {
                                    Value = val.total_first - val.total_last,
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<ElectricConsumptionDetail>
                        (@"SELECT * FROM ""electric_consumption"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC"
                        , new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = energyConsumption
                           .GroupBy(d => new
                           {
                               d.Bucket.Year
                           })
                           .Select(g => new
                           {
                               date_group = new DateTime(g.Key.Year, 1, 1),
                               total_first = g.Sum(d => Convert.ToDecimal(d.FirstValue)),
                               total_last = g.Sum(d => Convert.ToDecimal(d.LastValue)),
                           }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Data = groupedQuerys.Select(val => new ElectricDto
                                {
                                    Value = val.total_first - val.total_last,
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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<ElectricConsumptionDetail>
                        (@"SELECT * FROM ""electric_consumption"" WHERE id = @vid
                        AND date_trunc('week', bucket) = date_trunc('week', now()) 
                        ORDER BY id DESC, bucket DESC", new { vid = Vid });

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllElectricGeneratorConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                             new GetAllElectricGeneratorConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = energyConsumption.Select(val => new ElectricDto
                                 {
                                     Value = Convert.ToDecimal(val.FirstValue) - Convert.ToDecimal(val.LastValue),
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
        public async Task<GetAllEnergyConsumptionDto> GetAllEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => machine_id == m.MachineId && m.Subject.Vid.Contains("POWER-CONSUMPTION")).ToListAsync();

            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllEnergyConsumptionDto();

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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                        (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                        AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                        ORDER day_bucket DESC",
                        new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var total = energyConsumption.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => k.LastValue),
                            first = g.Sum(p => p.FirstValue),
                        }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionDto
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
                             new GetAllEnergyConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = total.Select(val => new EnergyDto
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

                        //var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        //{
                        //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                        //    first = g.Select(p => p.FirstValue).First()
                        //}).ToList();

                        var groupedQuerys = energyConsumption
                        .GroupBy(d => new
                        {
                            d.DayBucket.Month,
                            d.DayBucket.Year
                        })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_last = g.Sum(d => d.LastValue),
                              total_first = g.Sum(d => d.FirstValue),
                          }).ToList();



                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionDto
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
                             new GetAllEnergyConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = groupedQuerys.Select(val => new EnergyDto
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

                        //var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        //{
                        //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                        //    first = g.Select(p => p.FirstValue).First()
                        //}).ToList();

                        var groupedQuerys = energyConsumption
                        .GroupBy(d => new
                        {
                            //o.DateTime.Year,
                            //o.DateTime.Month,
                            WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.DayBucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                        })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                              total_first = g.Sum(d => Convert.ToDecimal(d.FirstValue)),
                              total_last = g.Sum(d => Convert.ToDecimal(d.LastValue)),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionDto
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
                            new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new EnergyDto
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

                        //var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                        //{
                        //    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                        //    last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                        //    first = g.Select(p => p.FirstValue).First()
                        //}).ToList();

                        var groupedQuerys = energyConsumption
                          .GroupBy(d => new
                          {
                              d.DayBucket.Year
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, 1, 1),
                              total_first = g.Sum(d => d.FirstValue),
                              total_last = g.Sum(d => d.LastValue),
                          }).ToList();

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionDto
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
                            new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Maximum = setting.Maximum,
                                Medium = setting.Medium,
                                Minimum = setting.Minimum,
                                Data = groupedQuerys.Select(val => new EnergyDto
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

                    //var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                    //(@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                    //AND date_trunc('month', bucket) = date_trunc('month', now()) 
                    //ORDER BY bucket ASC", new { vid = Vid });
                    var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                    (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @vid
                    AND date_trunc('week', day_bucket) = date_trunc('week', now()) 
                    ORDER BY day_bucket DESC", new { vid = Vid });

                    var totals = energyConsumptions.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day }).Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            last = g.Sum(k => Convert.ToDecimal(k.LastValue)),
                            first = g.Select(p => p.FirstValue).First()
                        }).ToList();

                        if (energyConsumptions.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionDto
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
                             new GetAllEnergyConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Maximum = setting.Maximum,
                                 Medium = setting.Medium,
                                 Minimum = setting.Minimum,
                                 Data = totals.Select(val => new EnergyDto
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
        public async Task<GetAllFrequencyInverterDto> GetAllFrequencyInverter(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject).Where(m => machine_id == m.MachineId
            && m.Subject.Vid.Contains("FRQ_INVERT")).ToListAsync();
            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllFrequencyInverterDto();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<FrqConsumption>
                        (@"SELECT * FROM ""frequency_inverter_consumption"" WHERE id = @vid
                         AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                         AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        decimal Total = consumptionBucket.Where(p => p.Id.Contains("FRQ_INVERT")).Sum(o => Convert.ToDecimal(o.Value));

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                DateTime = DateTime.UtcNow,
                                Value = Total

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

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<FrqConsumption>
                        (@"SELECT * FROM ""frequency_inverter_consumption"" WHERE id = @vid
                         AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = consumptionBucket
                         .GroupBy(d => new
                         {
                             //o.DateTime.Year,
                             //o.DateTime.Month,
                             WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                         })
                         .Select(g => new
                         {
                             date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                             total_frq = g.Where(p => p.Id.Contains("FRQ_INVERT")).Sum(o => Convert.ToDecimal(o.Value)),
                         }).ToList();

                        decimal Total = groupedQuerys.Select(o => o.total_frq).FirstOrDefault();

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                DateTime = DateTime.Now,
                                Value = Total

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

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<FrqConsumption>
                        (@"SELECT * FROM ""frequency_inverter_consumption"" WHERE id = @vid
                         AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                         AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = consumptionBucket
                         .GroupBy(d => new
                         {
                             d.Bucket.Month,
                             d.Bucket.Year,
                         })
                         .Select(g => new
                         {
                             date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                             total_frq = g.Where(p => p.Id.Contains("FRQ_INVERT")).Sum(o => Convert.ToDecimal(o.Value)),
                         }).ToList();

                        decimal Total = groupedQuerys.Select(o => o.total_frq).FirstOrDefault();

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                DateTime = DateTime.Now,
                                Value = Total

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

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<FrqConsumption>
                        (@"SELECT * FROM ""frequency_inverter_consumption"" WHERE id = @vid
                         AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                         AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = Vid, starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = consumptionBucket
                         .GroupBy(d => new
                         {
                             d.Bucket.Year,
                         })
                         .Select(g => new
                         {
                             date_group = new DateTime(g.Key.Year, 1, 1),
                             total_frq = g.Where(p => p.Id.Contains("FRQ_INVERT")).Sum(o => Convert.ToDecimal(o.Value)),
                         }).ToList();

                        decimal Total = groupedQuerys.Select(o => o.total_frq).FirstOrDefault();

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                DateTime = DateTime.Now,
                                Value = Total

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
                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<FrqConsumption>
                       (@"SELECT * FROM ""frequency_inverter_consumption"" WHERE id = @vid
                         AND date_trunc('day', bucket) = date_trunc('day', now()) 
                         ORDER BY id DESC, bucket DESC", new { vid = Vid });

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {
                            data =
                            new GetAllFrequencyInverterDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                DateTime = DateTime.Now,
                                Value = consumptionBucket.Sum(o => Convert.ToDecimal(o.Value))
                            };
                        }
                    }
                    break;
            }
            return data;
        }
        public async Task<GetAllTotalProductionDto> GetAllTotalProduction(Guid machine_id, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => machine_id == m.MachineId && m.Subject.Vid.Contains("COUNT-PRDCT")).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllTotalProductionDto();
            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                         AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

                        decimal TotalOk = consumptionBucket.Where(p => p.Id.Contains("COUNT-PRDCT-OK")).Sum(o => Convert.ToDecimal(o.LastValue));
                        decimal TotalNg = consumptionBucket.Where(p => p.Id.Contains("COUNT-PRDCT-NG")).Sum(o => Convert.ToDecimal(o.LastValue));

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                ValueOkTotal = TotalOk,
                                ValueNgTotal = TotalNg,
                                ValueOKPresentase = Math.Round((TotalOk / (TotalOk + TotalNg)) * 100, 2),
                                ValueNgPresentase = Math.Round((TotalNg / (TotalNg + TotalOk)) * 100, 2),
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

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = consumptionBucket
                          .GroupBy(d => new
                          {
                              //o.DateTime.Year,
                              //o.DateTime.Month,
                              WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
                              total_ok = g.Where(p => p.Id.Contains("COUNT-PRDCT-OK")).Sum(o => Convert.ToDecimal(o.LastValue)),
                              total_ng = g.Where(p => p.Id.Contains("COUNT-PRDCT-NG")).Sum(o => Convert.ToDecimal(o.LastValue)),

                          }).ToList();

                        decimal TotalOk = groupedQuerys.Select(o => o.total_ok).FirstOrDefault();
                        decimal TotalNg = groupedQuerys.Select(p => p.total_ng).FirstOrDefault();

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                ValueOkTotal = TotalOk,
                                ValueNgTotal = TotalNg,
                                ValueOKPresentase = Math.Round((TotalOk / (TotalOk + TotalNg)) * 100, 2),
                                ValueNgPresentase = Math.Round((TotalNg / (TotalNg + TotalOk)) * 100, 2),
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

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                         AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = consumptionBucket
                          .GroupBy(d => new
                          {
                              d.Bucket.Month,
                              d.Bucket.Year,
                          })
                          .Select(g => new
                          {
                              date_group = new DateTime(g.Key.Year, g.Key.Month, 1),
                              total_ok = g.Where(p => p.Id.Contains("COUNT-PRDCT-OK")).Sum(o => Convert.ToDecimal(o.LastValue)),
                              total_ng = g.Where(p => p.Id.Contains("COUNT-PRDCT-NG")).Sum(o => Convert.ToDecimal(o.LastValue)),

                          }).ToList();

                        decimal TotalOk = groupedQuerys.Select(o => o.total_ok).FirstOrDefault();
                        decimal TotalNg = groupedQuerys.Select(p => p.total_ng).FirstOrDefault();

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                ValueOkTotal = TotalOk,
                                ValueNgTotal = TotalNg,
                                ValueOKPresentase = Math.Round((TotalOk / (TotalOk + TotalNg)) * 100, 2),
                                ValueNgPresentase = Math.Round((TotalNg / (TotalNg + TotalOk)) * 100, 2),
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

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                         AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

                        var groupedQuerys = consumptionBucket
                        .GroupBy(d => new
                        {
                            d.Bucket.Year,
                        })
                        .Select(g => new
                        {
                            date_group = new DateTime(g.Key.Year, 1, 1),
                            total_ok = g.Where(p => p.Id.Contains("COUNT-PRDCT-OK")).Sum(o => Convert.ToDecimal(o.LastValue)),
                            total_ng = g.Where(p => p.Id.Contains("COUNT-PRDCT-NG")).Sum(o => Convert.ToDecimal(o.LastValue)),

                        }).ToList();

                        decimal TotalOk = groupedQuerys.Select(o => o.total_ok).FirstOrDefault();
                        decimal TotalNg = groupedQuerys.Select(p => p.total_ng).FirstOrDefault();

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                ValueOkTotal = TotalOk,
                                ValueNgTotal = TotalNg,
                                ValueOKPresentase = Math.Round((TotalOk / (TotalOk + TotalNg)) * 100, 2),
                                ValueNgPresentase = Math.Round((TotalNg / (TotalNg + TotalOk)) * 100, 2),
                            };
                        }
                    }
                    break;
                default:

                    var productConsumption = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                    (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY bucket DESC",
                    new { vid = vids, now = DateTime.Now.Date });

                    decimal valueOK = productConsumption.Where(k => k.Id.Contains("COUNT-PRDCT-OK")).Sum(o => Convert.ToDecimal(o.LastValue));
                    decimal valueNG = productConsumption.Where(k => k.Id.Contains("COUNT-PRDCT-NG")).Sum(o => Convert.ToDecimal(o.LastValue));

                    if (productConsumption.Count() == 0)
                    {
                        data = new GetAllTotalProductionDto
                        {
                            MachineName = machineName,
                            SubjectName = subjectName,
                        };
                    }
                    else
                    {

                        data =
                        new GetAllTotalProductionDto
                        {
                            MachineName = machineName,
                            SubjectName = subjectName,
                            ValueOkTotal = valueOK,
                            ValueNgTotal = valueNG,
                            ValueOKPresentase = Math.Round((valueOK / (valueOK + valueNG)) * 100, 2),
                            ValueNgPresentase = Math.Round((valueNG / (valueNG + valueOK)) * 100, 2),
                        };
                    }
                    break;
            }
            return data;
        }
        public async Task<List<GetListQualityCoolantFilingDto>> GetAllListQualityCoolantFiling(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityCoolantFilingDto> dt = new List<GetListQualityCoolantFilingDto>();
            var data = new GetListQualityCoolantFilingDto();

            var volumeVid = machine.Where(m => m.Subject.Vid.Contains("VOL-COLN")).FirstOrDefault();
            var barcodeVid = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();

            switch (type)
            {
                case "day":

                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;

                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);

                                dt.Add(listQuality);

                            }
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
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;

                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);

                                dt.Add(listQuality);

                            }
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
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;

                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);

                                dt.Add(listQuality);

                            }
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
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", 
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;

                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
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
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                        new { vid = volumeVid.Subject.Vid, now = DateTime.Now.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, now = DateTime.Now.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;

                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }
            return dt;
        }
        public async Task<List<GetListQualityMainLineDto>> GetAllListQualityMainLine(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityMainLineDto> dt = new List<GetListQualityMainLineDto>();
            var data = new GetListQualityMainLineDto();

            var timeVid = machine.Where(m => m.Subject.Vid.Contains("TIME-OPARATION")).FirstOrDefault();
            var frqVid = machine.Where(m => m.Subject.Vid.Contains("FRQ_INVERT")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = timeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (timeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = DateTime.Now,
                                FrqInverter = 0,
                                DurationStop = 0,

                            };
                        }
                        else
                        {

                            foreach (var f in timeConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (frQ != null)
                                {
                                    listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                }
                                var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                if (inverter != null)
                                {
                                    listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
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
                        var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = timeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (timeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = DateTime.Now,
                                FrqInverter = 0,
                                DurationStop = 0,

                            };
                        }
                        else
                        {

                            foreach (var f in timeConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (frQ != null)
                                {
                                    listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                }
                                var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                if (inverter != null)
                                {
                                    listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
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
                        var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = timeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (timeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = DateTime.Now,
                                FrqInverter = 0,
                                DurationStop = 0,

                            };
                        }
                        else
                        {

                            foreach (var f in timeConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (frQ != null)
                                {
                                    listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                }
                                var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                if (inverter != null)
                                {
                                    listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
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
                        var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", 
                        new { vid = timeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", 
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (timeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = DateTime.Now,
                                FrqInverter = 0,
                                DurationStop = 0,

                            };
                        }
                        else
                        {

                            foreach (var f in timeConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (frQ != null)
                                {
                                    listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                }
                                var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                if (inverter != null)
                                {
                                    listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
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
                        var timeConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = timeVid.Subject.Vid, now = DateTime.Now.Date });

                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = frqVid.Subject.Vid, now = DateTime.Now.Date });

                        if (timeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = DateTime.Now,
                                FrqInverter = 0,
                                DurationStop = 0,

                            };
                        }
                        else
                        {

                            foreach (var f in timeConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (frQ != null)
                                {
                                    listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                }
                                var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                if (inverter != null)
                                {
                                    listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
                        }
                    }
                break;
            }
            return dt;
        }
        public async Task<List<GetListQualityNutRunnerSteeringStemDto>> GetAllListQualityNutRunnerStem(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityNutRunnerSteeringStemDto> dt = new List<GetListQualityNutRunnerSteeringStemDto>();
            var data = new GetListQualityNutRunnerSteeringStemDto();

            var bcVid = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
            var statusVid = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
            var torsiVid = machine.Where(m => m.Subject.Vid.Contains("TORQ")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {

                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
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
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {

                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
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
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {

                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
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
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {

                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
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
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC", new { vid = bcVid.Subject.Vid, now = DateTime.Now.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC", new { vid = torsiVid.Subject.Vid, now = DateTime.Now.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC", new { vid = statusVid.Subject.Vid, now = DateTime.Now.Date, });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {

                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }
            return dt;
        }
        public async Task<List<GetListQualityOilBrakeDto>> GetAllListQualityOilBrake(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityOilBrakeDto> dt = new List<GetListQualityOilBrakeDto>();
            var data = new GetListQualityOilBrakeDto();

            var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
            var leak = machine.Where(m => m.Subject.Vid.Contains("LEAK-TES")).FirstOrDefault();
            var vol = machine.Where(m => m.Subject.Vid.Contains("VOL-OIL-BRAEK")).FirstOrDefault();
            var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
            var code = machine.Where(m => m.Subject.Vid.Contains("CODE")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = bc.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var leakTesterConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = leak.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var volumeOilConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = vol.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = status.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var codeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = code.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityOilBrakeDto
                            {
                                DateTime = DateTime.Now,
                                DataBarcode = "-",
                                LeakTester = 0,
                                VolumeOilBrake = 0,
                                Status = "-",
                                ErrorCode = 0


                            };
                        }
                        else
                        {

                            foreach (var s in statusConsumption)
                            {
                                GetListQualityOilBrakeDto listQuality = new GetListQualityOilBrakeDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }
                                var leakTester = leakTesterConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (leakTester != null)
                                {
                                    listQuality.LeakTester = Convert.ToDecimal(leakTester.Value);
                                }
                                var volumeOil = volumeOilConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (volumeOil != null)
                                {
                                    listQuality.VolumeOilBrake = Convert.ToDecimal(volumeOil.Value);
                                }
                                var errorCode = codeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (errorCode != null)
                                {
                                    listQuality.ErrorCode = Convert.ToInt32(errorCode.Value);
                                }
                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
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
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                    (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY  bucket DESC",
                    new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                        var leakTesterConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY  bucket DESC",
                        new { vid = leak.Subject.Vid, now = DateTime.Now.Date });

                        var volumeOilConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY  bucket DESC",
                        new { vid = vol.Subject.Vid, now = DateTime.Now.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                    ORDER BY  bucket DESC",
                        new { vid = status.Subject.Vid, dateNow = DateTime.Now.Date, });

                        var codeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                    ORDER BY  bucket DESC",
                        new { vid = code.Subject.Vid, dateNow = DateTime.Now.Date, });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityOilBrakeDto
                            {
                                DateTime = DateTime.Now,
                                DataBarcode = "-",
                                LeakTester = 0,
                                VolumeOilBrake = 0,
                                Status = "-",
                                ErrorCode = 0


                            };
                        }
                        else
                        {

                            foreach (var s in statusConsumption)
                            {
                                GetListQualityOilBrakeDto listQuality = new GetListQualityOilBrakeDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }
                                var leakTester = leakTesterConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (leakTester != null)
                                {
                                    listQuality.LeakTester = Convert.ToDecimal(leakTester.Value);
                                }
                                var volumeOil = volumeOilConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (volumeOil != null)
                                {
                                    listQuality.VolumeOilBrake = Convert.ToDecimal(volumeOil.Value);
                                }
                                var errorCode = codeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (errorCode != null)
                                {
                                    listQuality.ErrorCode = Convert.ToInt32(errorCode.Value);
                                }
                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
                        }
                    }
                    break;
            }
            return dt;
        }
        public async Task<List<GetListQualityPressConeRaceDto>> GetAllListQualityPressConeRace(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();


            List<GetListQualityPressConeRaceDto> dt = new List<GetListQualityPressConeRaceDto>();
            var data = new GetListQualityPressConeRaceDto();

            var kedalaman = machine.Where(m => m.Subject.Vid.Contains("DEPTH")).FirstOrDefault();
            var tonase = machine.Where(m => m.Subject.Vid.Contains("TONASE")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var kedalamanConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = kedalaman.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var tonaseConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                            new { vid = tonase.Subject.Vid, starttime = start.Date, endtime = end.Date });
                        if (kedalamanConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityPressConeRaceDto
                            {
                                DateTime = DateTime.Now,
                                Kedalaman = 0,
                                Tonase = 0

                            };
                        }
                        else
                        {

                            foreach (var s in kedalamanConsumption)
                            {
                                GetListQualityPressConeRaceDto listQuality = new GetListQualityPressConeRaceDto();

                                var Depth = tonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Depth != null)
                                {
                                    listQuality.Kedalaman = Convert.ToDecimal(s.Value);
                                }
                                var Tonasee = kedalamanConsumption.Where(k => k.Bucket == Depth.Bucket).FirstOrDefault();
                                if (Tonasee != null)
                                {
                                    listQuality.Tonase = Convert.ToDecimal(Tonasee.Value);
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
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
                        var kedalamanConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = kedalaman.Subject.Vid, now = DateTime.Now.Date });

                        var tonaseConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = tonase.Subject.Vid, now = DateTime.Now.Date });


                        if (kedalamanConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityPressConeRaceDto
                            {
                                DateTime = DateTime.Now,
                                Kedalaman = 0,
                                Tonase = 0

                            };
                        }
                        else
                        {

                            foreach (var s in kedalamanConsumption)
                            {
                                GetListQualityPressConeRaceDto listQuality = new GetListQualityPressConeRaceDto();

                                var Depth = tonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Depth != null)
                                {
                                    listQuality.Kedalaman = Convert.ToDecimal(s.Value);
                                }
                                var Tonasee = kedalamanConsumption.Where(k => k.Bucket == Depth.Bucket).FirstOrDefault();
                                if (Tonasee != null)
                                {
                                    listQuality.Tonase = Convert.ToDecimal(Tonasee.Value);
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
                        }
                    }
                    break;
            }

            return dt;
        }
        public async Task<List<GetListQualityRobotScanImageDto>> GetAllListQualityRobotScanImage(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityRobotScanImageDto> dt = new List<GetListQualityRobotScanImageDto>();
            var data = new GetListQualityRobotScanImageDto();

            var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
            var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                        (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = bc.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                            (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                            new { vid = status.Subject.Vid, starttime = start.Date, endtime = end.Date });
                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityRobotScanImageDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var s in statusConsumption)
                            {
                                GetListQualityRobotScanImageDto listQuality = new GetListQualityRobotScanImageDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
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
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                        (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                                (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                        ORDER BY  bucket DESC",
                                new { vid = status.Subject.Vid, dateNow = DateTime.Now.Date, });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityRobotScanImageDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",

                            };
                        }
                        else
                        {

                            foreach (var s in statusConsumption)
                            {
                                GetListQualityRobotScanImageDto listQuality = new GetListQualityRobotScanImageDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);

                            }
                        }
                        break;

                    }
            }
            return dt;
        }

        public async Task<GetAllMachineInformationDto> GetAllMachineInformationAsync(Guid machine_id)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
               .Where(m => (machine_id == m.MachineId && m.Subject.Vid.Contains("CYCLE-COUNT"))
               || (machine_id == m.MachineId && m.Subject.Vid.Contains("RUN-TIME"))
               || (machine_id == m.MachineId && m.Subject.Vid.Contains("RIM"))).ToListAsync();


            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllMachineInformationDto();

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
                data = new GetAllMachineInformationDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,

                };

            }
            else
            {


                data = new GetAllMachineInformationDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = DateTime.Now,
                    ValueRunning = categorys.Select(c => Convert.ToDecimal(c.LastCycleCount?.Value)).FirstOrDefault(),
                    CycleCount = categorys.Select(x => Convert.ToDecimal(x.LastRunTime?.Value)).Skip(1).FirstOrDefault(),
                    LastTimeCalibration = categorys.Select(n => n.LastKalibrasi?.Value).Skip(2).FirstOrDefault(),

                };
            }
            return data;
        }
        //Energy Consumption Summary yang Di Machine Information
        public async Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(string type, DateTime start, DateTime end)
        {
            var querys = from shm in _dbContext.subjectHasMachines 
                         join s in _dbContext.Subject on shm.SubjectId equals s.Id
                         join m in _dbContext.Machines on shm.MachineId equals m.Id
                         where s.Vid.Contains("POWER-CONSUMPTION")
                         select new { Machine = m, Subject = s, SubjectMachine = shm };

            var result = await querys.ToListAsync();

            List<GetAllDetailEnergyConsumptionDto> dt = new List<GetAllDetailEnergyConsumptionDto>();
            var data = new GetAllDetailEnergyConsumptionDto();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                       var EnergyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                       (@"SELECT * FROM ""power_consumption_setting"" WHERE id = ANY(@vid)
                       AND date_trunc('day', day_bucket) >= date_trunc('day', @starttime::date)
                       AND date_trunc('day', day_bucket) <= date_trunc('day', @endtime::date)
                       ORDER BY day_bucket DESC",
                       new { vid = querys.Select(o => o.Subject.Vid).ToList(), starttime = start.Date, endtime = end.Date });

                        var Groups = EnergyConsumptions.GroupBy(p => new { p.DayBucket.Year, p.DayBucket.Month, p.DayBucket.Day })
                        .Select(g => new
                        {
                            date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            total = g.Sum(k => k.Value)
                        }).ToList();

                        if (EnergyConsumptions.Count() == 0)
                        {
                            data =
                            new GetAllDetailEnergyConsumptionDto
                            {
                                ValueKwh = 0,
                                ValueCo2 = 0,
                                Label = "",
                                DateTime = DateTime.Now
                            };
                            dt.Add(data);
                        }
                        else
                        {
                            dt = Groups.Select(o => new GetAllDetailEnergyConsumptionDto
                            {
                                ValueKwh = o.total,
                                ValueCo2 = Math.Round((o.total) * Convert.ToDecimal(0.87), 2),
                                Label = o.date_time.ToString("dd-MM-yy").ToString(),
                                DateTime = DateTime.Now,

                            }).ToList(); 
                           
                           // dt.Add(data);
                        }
                    }
                    break;

                default:
                var EnergyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionTop>
                         (@"SELECT * FROM ""power_consumption_setting"" WHERE id = ANY(@vid)
                         AND date_trunc('week', bucket::date) = date_trunc('week', @now)
                         ORDER BY value DESC",
                         new { vid = querys.Select(o => o.Subject.Vid).ToList(), now = DateTime.Now.Date });


                if (EnergyConsumption.Count() == 0)
                {
                    data =
                    new GetAllDetailEnergyConsumptionDto
                    {
                        ValueKwh = 0,
                        ValueCo2 = 0,
                        Label = "",
                        DateTime = DateTime.Now
                    };
                    dt.Add(data);
                }
                else
                {
                        dt = EnergyConsumption.Select(o => new GetAllDetailEnergyConsumptionDto
                        {
                            ValueKwh = o.Value,
                            ValueCo2 = Math.Round((o.Value) * Convert.ToDecimal(0.87), 2),
                            Label = o.DayBucket.ToString("dd-MM-yy").ToString(),
                            DateTime = DateTime.Now,

                        }).ToList();
                        // dt.Add(data);
                    }
                    break;
            }
            return dt;
        }

    }
}

