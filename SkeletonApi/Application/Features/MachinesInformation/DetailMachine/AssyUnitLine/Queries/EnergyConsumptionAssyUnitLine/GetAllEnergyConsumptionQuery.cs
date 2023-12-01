using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Globalization;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine
{
    public record GetAllEnergyConsumptionQuery : IRequest<Result<GetAllEnergyConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }

    internal class GetAllEnergyConsumptionHandler : IRequestHandler<GetAllEnergyConsumptionQuery, Result<GetAllEnergyConsumptionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;

        public GetAllEnergyConsumptionHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }

        public async Task<Result<GetAllEnergyConsumptionDto>> Handle(GetAllEnergyConsumptionQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("PWM-KWH")).ToListAsync();

            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllEnergyConsumptionDto();

            switch (query.Type)
            {
                case "month":
                    if (query.End.Date < query.Start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                         AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                         AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = Vid, starttime = query.Start.Date, endtime = query.End.Date });

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
                            data = new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {
                            data =
                             new GetAllEnergyConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = groupedQuerys.Select(val => new EnergyDto
                                 {
                                     ValueKwh = val.total_first - val.total_last,
                                     ValueCo2 = Math.Round(val.total_first - val.total_last * Convert.ToDecimal(0.87), 2),
                                     Label = val.date_group.AddHours(7).ToString("MMM"),
                                     DateTime = val.date_group,
                                 }).OrderByDescending(x => x.DateTime).ToList()

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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                         AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                         AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = Vid, starttime = query.Start.Date, endtime = query.End.Date });

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
                            data = new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Data = groupedQuerys.Select(val => new EnergyDto
                                {
                                    ValueKwh = val.total_first - val.total_last,
                                    ValueCo2 = Math.Round(val.total_first - val.total_last * Convert.ToDecimal(0.87), 2),
                                    Label = "Week " + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(val.date_group, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(),
                                    DateTime = val.date_group,
                                }).OrderByDescending(x => x.DateTime).ToList()

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
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                         AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                         AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = Vid, starttime = query.Start.Date, endtime = query.End.Date });

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
                            data = new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                            new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                                Data = groupedQuerys.Select(val => new EnergyDto
                                {
                                    ValueKwh = val.total_first - val.total_last,
                                    ValueCo2 = Math.Round(val.total_first - val.total_last * Convert.ToDecimal(0.87), 2),
                                    Label = val.date_group.AddHours(7).ToString("yyy"),
                                    DateTime = val.date_group,
                                }).OrderByDescending(x => x.DateTime).ToList()

                            };
                        }
                    }
                    break;
                default:
                    if (query.End.Date < query.Start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumptionDetail>
                        (@"SELECT * FROM ""energy_consumption"" WHERE id = @vid
                         AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                         AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                         ORDER BY id DESC, bucket DESC", new { vid = Vid, starttime = query.Start.Date, endtime = query.End.Date });

                        if (energyConsumption.Count() == 0)
                        {
                            data = new GetAllEnergyConsumptionDto
                            {
                                MachineName = machineName,
                                SubjectName = subjectName,
                            };
                        }
                        else
                        {

                            data =
                             new GetAllEnergyConsumptionDto
                             {
                                 MachineName = machineName,
                                 SubjectName = subjectName,
                                 Data = energyConsumption.Select(val => new EnergyDto
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

            return await Result<GetAllEnergyConsumptionDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

