using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetAllMachine
{
    public record GetAllMachineMaintPreventiveQuery : IRequest<Result<List<GetAllMAchineMaintPreventiveDto>>>
    {
        [JsonPropertyName("start_date")]
        public DateOnly? start_date { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly? end_date { get; set; }

        public GetAllMachineMaintPreventiveQuery(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null || enddate == null)
            {
                start_date = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
                end_date = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                start_date = DateOnly.FromDateTime((DateTime)startdate);
                end_date = DateOnly.FromDateTime((DateTime)enddate);
            }
        }
    }

    internal class GetAllMachineMaintPreventiveQueryHandle : IRequestHandler<GetAllMachineMaintPreventiveQuery, Result<List<GetAllMAchineMaintPreventiveDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMachineMaintPreventiveQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllMAchineMaintPreventiveDto>>> Handle(GetAllMachineMaintPreventiveQuery query, CancellationToken cancellationToken)
        {
            var data = new List<GetAllMAchineMaintPreventiveDto>();

            var sql = _unitOfWork.Data<MaintenacePreventive>().Entities
                            .Where(x => x.StartDate >= query.start_date && x.StartDate <= query.end_date);
            data = await sql.GroupBy(x => new { x.StartDate })
                            .Select(c => 
                            new GetAllMAchineMaintPreventiveDto
                            { 
                                CountActual = c.Count(j => j.EndDate != null),
                                CountPlan = c.Count(j => j.StartDate != null),
                                label = new DateTime(c.Key.StartDate.Value.Year, c.Key.StartDate.Value.Month, c.Key.StartDate.Value.Day).ToString("dd MMM", new CultureInfo("en-US")),
                                StartDate = c.Key.StartDate.Value,
                            })
                            //.ProjectTo<GetAllMAchineMaintPreventiveDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

            return await Result<List<GetAllMAchineMaintPreventiveDto>>.SuccessAsync(data, "Successfully fetch data");
        }
    }
}
