using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption;
using SkeletonApi.Application.Features.Dummys.DummyDto;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption
{
    public record GetAllElectricGeneratorConsumptionQuery : IRequest<Result<GetAllElectricGeneratorConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllElectricGeneratorConsumptionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllElectricGeneratorConsumptionHandler : IRequestHandler<GetAllElectricGeneratorConsumptionQuery, Result<GetAllElectricGeneratorConsumptionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllElectricGeneratorConsumptionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<GetAllElectricGeneratorConsumptionDto>> Handle(GetAllElectricGeneratorConsumptionQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("ELECT_GNTR")).ToListAsync();
            string vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();
            var data = new GetAllElectricGeneratorConsumptionDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities.Where(c => vid.Contains(c.Id)).Select(g =>
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
                data = new GetAllElectricGeneratorConsumptionDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    Data = new List<ElectricDto>(),
                };
            }
            else
            {
                data =
                    new GetAllElectricGeneratorConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        Data = categorys.Select(val => new ElectricDto
                        {
                            Value = Convert.ToDecimal(val.Value),
                            Label = val.DateTime.AddHours(7).ToString("HH:mm:ss"),
                            DateTime = val.DateTime,
                        }).ToList()
                    };
            }

            return await Result<GetAllElectricGeneratorConsumptionDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
