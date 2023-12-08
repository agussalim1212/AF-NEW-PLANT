using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetDetail
{
    public record GetDetailMaintPreventiveQuery : IRequest<Result<List<GetDetailMaintPreventiveDto>>> 
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } 

        public GetDetailMaintPreventiveQuery(Guid id)
        {
            Id = id;
        }
    }
    internal class GetDetailMaintPreventiveQueryHandle : IRequestHandler<GetDetailMaintPreventiveQuery, Result<List<GetDetailMaintPreventiveDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDetailMaintPreventiveQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetDetailMaintPreventiveDto>>> Handle(GetDetailMaintPreventiveQuery query, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Repository<MaintenacePreventive>().Entities
                .Where(x => x.Id == query.Id)
                .Join(
                    _unitOfWork.Repository<Machine>().Entities,
                    preventive => preventive.MachineId,
                    machine => machine.Id,
                    (preventive, machine) => new GetDetailMaintPreventiveDto
                    {
                        Name = machine.Name,
                        Plan = preventive.Plan,
                        StartDate = preventive.StartDate,
                        Actual = preventive.Actual,
                        EndDate = preventive.EndDate,
                    }
                )
                .ToListAsync(cancellationToken);

            if (entity != null && entity.Any())
            {
                return Result<List<GetDetailMaintPreventiveDto>>.Success(entity, "Successfully fetch data");
            }

            return Result<List<GetDetailMaintPreventiveDto>>.Failure("Data Not Found");
        }

    }
}
