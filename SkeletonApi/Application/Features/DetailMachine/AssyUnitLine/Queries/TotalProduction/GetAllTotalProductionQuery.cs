using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.Dummys.DummyDto;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction
{
    public class GetAllTotalProductionQuery : IRequest<Result<GetAllTotalProductionDto>>
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

        public GetAllTotalProductionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllTotalProductionDto>> Handle(GetAllTotalProductionQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("COUNT-PRDCT")).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllTotalProductionDto();
            decimal totalOK = 0;
            decimal totalNG = 0;

            var categorys = await _unitOfWork.Data<Dummy>().Entities.Where(c => vids.Contains(c.Id)).Select(g =>
                new
                {
                    Id = g.Id,
                    DateTime = g.DateTime,
                    ValueOkTotal = g.Id.Contains("COUNT-PRDCT-OK") ? Convert.ToDecimal(g.Value) : 0,
                    ValueNgTotal = g.Id.Contains("COUNT-PRDCT-NG") ? Convert.ToDecimal(g.Value) : 0,

                }).GroupBy(c => c.Id).Select(o => new
                {
                    resultOk = (o.Max(x => x.ValueOkTotal)),
                    resultNg = (o.Max(x => x.ValueNgTotal)),
                }).ToListAsync();

            if (categorys.Count() == 0)
            {
                var category = await _unitOfWork.Data<Dummy>().Entities.Select(g =>
                new GetAllTotalProductionDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                })
                .ProjectTo<GetAllTotalProductionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            }
            else
            {
                foreach(var rs in categorys)
                {
                    totalOK += rs.resultOk;
                    totalNG += rs.resultNg;
                }

                var category = await _unitOfWork.Data<Dummy>().Entities.Select(g =>
                new GetAllTotalProductionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        ValueOkTotal = totalOK,
                        ValueNgTotal = totalNG,
                        ValueOKPresentase = Math.Round((totalOK / (totalOK + totalNG)) * 100, 2),
                        ValueNgPresentase = Math.Round((totalNG / (totalNG + totalOK)) * 100, 2),
                    })
                    .ProjectTo<GetAllTotalProductionDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

                data = category;
            }

            return await Result<GetAllTotalProductionDto>.SuccessAsync(data, "Successfully fetch data");
        }


    }
}
