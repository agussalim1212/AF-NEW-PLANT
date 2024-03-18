using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.TotalProduction;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;
using System.Globalization;

namespace SkeletonApi.Persistence.Repositories
{
    public class DetailGensubRepository : IDetailGensubRespository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IGenericRepository<Machine> _repositoryMachine;
        public DetailGensubRepository(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IGenericRepository<Machine> repositoryMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _repositoryMachine = repositoryMachine;
        }

       
        public async Task<GetAllTotalProductionGensubDto> GetAllTotalProductionGensubDto(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
           .Where(m => machineId == m.MachineId && m.Subject.Vid.Contains("COUNT-PRDCT")).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllTotalProductionGensubDto();

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
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

                        decimal TotalOk = consumptionBucket.Where(p => p.Id.Contains("COUNT-PRDCT-OK")).Sum(o => Convert.ToDecimal(o.LastValue));
                        decimal TotalNg = consumptionBucket.Where(p => p.Id.Contains("COUNT-PRDCT-NG")).Sum(o => Convert.ToDecimal(o.LastValue));

                        if (consumptionBucket.Count() == 0)
                        {
                            data = new GetAllTotalProductionGensubDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionGensubDto
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
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

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
                            data = new GetAllTotalProductionGensubDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionGensubDto
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
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

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
                            data = new GetAllTotalProductionGensubDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionGensubDto
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
                         ORDER BY id DESC, bucket DESC"
                        , new { vid = vids.ToList(), starttime = start.Date, endtime = end.Date });

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
                            data = new GetAllTotalProductionGensubDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllTotalProductionGensubDto
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
                        data = new GetAllTotalProductionGensubDto
                        {
                            MachineName = machineName,
                            SubjectName = subjectName,
                        };
                    }
                    else
                    {

                        data =
                        new GetAllTotalProductionGensubDto
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
    }
}


