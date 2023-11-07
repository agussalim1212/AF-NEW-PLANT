using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Dummys.DummyDto;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine
{
    public record GetAllEnergyConsumptionGensubQuery : IRequest<Result<GetAllEnergyConsumptionGensubDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionGensubQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllEnergyConsumptionGensubHandler : IRequestHandler<GetAllEnergyConsumptionGensubQuery, Result<GetAllEnergyConsumptionGensubDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllEnergyConsumptionGensubHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<GetAllEnergyConsumptionGensubDto>> Handle(GetAllEnergyConsumptionGensubQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId).FirstOrDefaultAsync();

            var data = new GetAllEnergyConsumptionGensubDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities.Where(c => machine.Subject.Vid == c.Id).Select(g =>
                new DummyDto
                {
                    Id = g.Id,
                    Value = g.Value,
                    DateTime = g.DateTime,
               
                })
                .ProjectTo<DummyDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (categorys.Count() == 0)
            {
                var category = await _unitOfWork.Data<Dummy>().Entities.Where(c => machine.Subject.Vid == c.Id).Select(g =>
                new GetAllEnergyConsumptionGensubDto
                {
                    MachineName = machine.Machine.Name,
                    SubjectName = machine.Subject.Subjects,
                })
                .ProjectTo<GetAllEnergyConsumptionGensubDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            }
            else
            {
                var category = await _unitOfWork.Data<Dummy>().Entities.Where(c => machine.Subject.Vid == c.Id).Select(g =>
                    new GetAllEnergyConsumptionGensubDto
                    {
                        MachineName = machine.Machine.Name,
                        SubjectName = machine.Subject.Subjects,
                        Data = categorys.Select(val => new EnergyGensubDto
                        {
                            ValueKwh = Convert.ToDecimal(val.Value),
                            ValueCo2 = Math.Round((Convert.ToDecimal(val.Value) * Convert.ToDecimal(0.87)),2),
                            Label = "senin",
                           // DateTime = DateTime.,




                        }).ToList()
                    })
                    .ProjectTo<GetAllEnergyConsumptionGensubDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                data = category;
            }
          //  var lucky = category;
            return await Result<GetAllEnergyConsumptionGensubDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

