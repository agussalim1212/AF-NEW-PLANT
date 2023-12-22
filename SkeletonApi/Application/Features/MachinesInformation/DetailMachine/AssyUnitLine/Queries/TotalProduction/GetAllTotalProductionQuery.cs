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
        private readonly IDetailAssyUnitRepository _detailAssyUnitRepository;

        public GetAllTotalProductionHandler(IDetailAssyUnitRepository detailAssyUnitRepository)
        {
           _detailAssyUnitRepository = detailAssyUnitRepository;
        }

        public async Task<Result<GetAllTotalProductionDto>> Handle(GetAllTotalProductionQuery query, CancellationToken cancellationToken)
        {
            var data = await _detailAssyUnitRepository.GetAllTotalProduction(query.MachineId, query.Type, query.Start, query.End);
            return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
        }


    }
}
