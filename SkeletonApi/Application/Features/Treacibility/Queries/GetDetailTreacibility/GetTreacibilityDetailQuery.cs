using AutoMapper.QueryableExtensions;
using AutoMapper;
using MediatR;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Domain.Entities.Tsdb;

namespace SkeletonApi.Application.Features.Treacibility.Queries.GetDetailTreacibility
{
    public record GetTreacibilityDetailQuery : IRequest<Result<GetTreacibilityDetailDto>>
    {

        public string EngineId { get; set; }
        public string Torsi { get; set; }

        public GetTreacibilityDetailQuery(string engineId, string torsi)
        {
            EngineId = engineId;
            Torsi = torsi;
        }
    }

    internal class GetTreacibilityDetailHandler : IRequestHandler<GetTreacibilityDetailQuery, Result<GetTreacibilityDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTreacibilityDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetTreacibilityDetailDto>> Handle(GetTreacibilityDetailQuery query, CancellationToken cancellationToken)
        {
            var Treacibility  = await _unitOfWork.Data<EnginePart>().Entities.Where(x => x.EngineId.ToUpper() == query.EngineId.ToUpper() && x.Torsi.ToUpper() == query.Torsi.ToUpper())
                            .ProjectTo<GetTreacibilityDetailDto>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);

            return await Result<GetTreacibilityDetailDto>.SuccessAsync(Treacibility, "Successfully fetch data");
        }
    }




}
