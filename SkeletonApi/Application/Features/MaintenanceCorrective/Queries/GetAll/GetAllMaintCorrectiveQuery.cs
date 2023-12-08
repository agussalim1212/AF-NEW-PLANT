using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetAllMachine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetAll
{
    public record GetAllMaintCorrectiveQuery : IRequest<Result<List<GetAllMaintCorrectiveDto>>>
    {
        [JsonPropertyName("start_date")]
        public DateOnly? start_date { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly? end_date { get; set; }

        public GetAllMaintCorrectiveQuery(DateTime? startdate, DateTime? enddate)
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

    internal class GetAllMaintCorrectiveQueryHandle : IRequestHandler<GetAllMaintCorrectiveQuery, Result<List<GetAllMaintCorrectiveDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMaintCorrectiveQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllMaintCorrectiveDto>>> Handle(GetAllMaintCorrectiveQuery query, CancellationToken cancellationToken)
        {
                var data = new List<GetAllMaintCorrectiveDto>();

                var sql = _unitOfWork.Data<MaintCorrective>().Entities
                   .Where(x => x.StartDate >= query.start_date && x.StartDate <= query.end_date);
                data = await sql.GroupBy(x => new { x.StartDate })
                                .Select(c =>
                                new GetAllMaintCorrectiveDto
                                {
                                    CountActual = c.Count(j => j.EndDate != null),
                                    label = new DateTime(c.Key.StartDate.Value.Year,
                                    c.Key.StartDate.Value.Month, c.Key.StartDate.Value.Day)
                                    .ToString("dd MMM", new CultureInfo("en-US")),
                                    StartDate = c.Key.StartDate.Value,
                                })
                                .ToListAsync(cancellationToken);

                return await Result<List<GetAllMaintCorrectiveDto>>.SuccessAsync(data, "Successfully fetch data");
        }
    }
}
