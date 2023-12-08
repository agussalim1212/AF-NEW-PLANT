using AutoMapper;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetDetail
{
    public record GetDetailMaintCorrectiveQuery : IRequest<Result<List<GetDetailMaintCorrectiveDto>>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        public GetDetailMaintCorrectiveQuery(Guid id)
        {
            Id = id;
        }
    }

    internal class GetDetailMaintCorrectiveQueryHandle : IRequestHandler<GetDetailMaintCorrectiveQuery, Result<List<GetDetailMaintCorrectiveDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDetailMaintCorrectiveQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetDetailMaintCorrectiveDto>>> Handle(GetDetailMaintCorrectiveQuery query, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Repository<MaintCorrective>().Entities
                .Where(x => x.Id == query.Id)
                .Join(
                    _unitOfWork.Repository<Machine>().Entities,
                    preventive => preventive.MachineId,
                    machine => machine.Id,
                    (preventive, machine) => new GetDetailMaintCorrectiveDto
                    {
                        Name = machine.Name,
                        StartDate = preventive.StartDate,
                        Actual = preventive.Actual,
                        EndDate = preventive.EndDate,
                    }
                )
                .ToListAsync(cancellationToken);

            if (entity != null && entity.Any())
            {
                return Result<List<GetDetailMaintCorrectiveDto>>.Success(entity, "Successfully fetch data");
            }

            return Result<List<GetDetailMaintCorrectiveDto>>.Failure("Data Not Found");
        }
    }
}
