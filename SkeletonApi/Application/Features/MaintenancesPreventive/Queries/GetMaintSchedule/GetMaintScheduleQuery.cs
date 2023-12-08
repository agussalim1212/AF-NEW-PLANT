using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetDetail;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetMaintSchedule
{
    public record GetMaintScheduleQuery : IRequest<Result<List<GetMaintScheduleDto>>>;

    internal class GetMaintScheduleQueryHandler : IRequestHandler<GetMaintScheduleQuery, Result<List<GetMaintScheduleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMaintScheduleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetMaintScheduleDto>>> Handle(GetMaintScheduleQuery query, CancellationToken cancellationToken)
        {
            DateOnly date_time = DateOnly.FromDateTime(DateTime.Now);
            DateOnly dateMinus = DateOnly.FromDateTime(DateTime.Now.Date.AddDays(+7));

            var Schedule = await _unitOfWork.Repository<MaintenacePreventive>().Entities
                            .Where(c => c.StartDate >= date_time && c.StartDate <= dateMinus && c.EndDate == null)
                            .OrderByDescending(c => c.StartDate)
                            .Join(
                                    _unitOfWork.Repository<Machine>().Entities,
                                    preventive => preventive.MachineId,
                                    machine => machine.Id,
                                    (preventive, machine) => new GetMaintScheduleDto
                                    {
                                        Id = preventive.Id,
                                        Name = machine.Name,
                                        Plan = preventive.Plan,
                                        StartDate = preventive.StartDate,
                                    }
                                )
                            //.ProjectTo<GetMaintScheduleDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

            return await Result<List<GetMaintScheduleDto>>.SuccessAsync(Schedule, "Successfully fetch data");
        }
    }
}
