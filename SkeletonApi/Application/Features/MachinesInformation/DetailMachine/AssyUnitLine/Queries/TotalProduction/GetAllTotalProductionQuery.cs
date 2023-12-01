using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Globalization;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction
{
    public record GetAllTotalProductionQuery : IRequest<Result<GetAllTotalProductionDto>>
    {
        
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public GetAllTotalProductionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }

    internal class GetAllTotalProductionHandler : IRequestHandler<GetAllTotalProductionQuery, Result<GetAllTotalProductionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;

        public GetAllTotalProductionHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }

        public async Task<Result<GetAllTotalProductionDto>> Handle(GetAllTotalProductionQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("COUNT-PRDCT")).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllTotalProductionDto();
            switch (query.Type)
            {
                 case "day":
                    if (query.End.Date < query.Start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                      
                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                         AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = query.Start.Date, endtime = query.End.Date });

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
                    if (query.End.Date < query.Start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = query.Start.Date, endtime = query.End.Date });

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
                    if (query.End.Date < query.Start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                         AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = query.Start.Date, endtime = query.End.Date });

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
                    if (query.End.Date < query.Start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {

                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                        (@"SELECT * FROM ""total_production_consumption"" WHERE id = ANY(@vid)
                         AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                         AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = query.Start.Date, endtime = query.End.Date });

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
            return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
        }


    }
}
