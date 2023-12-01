﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Globalization;

namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine
{
    public record GetAllEnergyConsumptionAssyWheelLineQuery : IRequest<Result<GetAllEnergyConsumptionAssyWheelLineDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionAssyWheelLineQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllEnergyConsumptionAssyWheelLineHandler : IRequestHandler<GetAllEnergyConsumptionAssyWheelLineQuery, Result<GetAllEnergyConsumptionAssyWheelLineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;

        public GetAllEnergyConsumptionAssyWheelLineHandler(IUnitOfWork unitOfWork, IMapper mapper, IDapperReadDbConnection dapperReadDbConnection)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }


        public async Task<Result<GetAllEnergyConsumptionAssyWheelLineDto>> Handle(GetAllEnergyConsumptionAssyWheelLineQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId
            && m.Subject.Vid.Contains("PWM-KWH")).ToListAsync();
            string Vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();


            var data = new GetAllEnergyConsumptionAssyWheelLineDto();

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

            return await Result<GetAllEnergyConsumptionAssyWheelLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

