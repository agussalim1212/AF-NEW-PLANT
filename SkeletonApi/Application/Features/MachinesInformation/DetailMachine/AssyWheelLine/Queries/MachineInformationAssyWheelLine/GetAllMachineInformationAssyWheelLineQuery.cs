using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine
{

    public record GetAllMachineInformationAssyWheelLineQuery : IRequest<Result<GetAllMachineInformationAssyWheelLineDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationAssyWheelLineQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetAllMachineInformationAssyWheelHandler : IRequestHandler<GetAllMachineInformationAssyWheelLineQuery, Result<GetAllMachineInformationAssyWheelLineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMachineInformationAssyWheelHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllMachineInformationAssyWheelLineDto>> Handle(GetAllMachineInformationAssyWheelLineQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("CYCLE-COUNT") || query.MachineId == m.MachineId && m.Subject.Vid.Contains("RUN-TIME")).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllMachineInformationAssyWheelLineDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities
              .Where(c => vids.Contains(c.Id))
              .GroupBy(c => c.Id)
              .Select(groups => new
              {
                  Id = groups.Key, // ID dari kelompok
                  LastRunTime = groups.Where(g => g.Id.Contains("RUN-TIME"))
                      .OrderByDescending(g => g.DateTime)
                      .FirstOrDefault(), // Get the last "Run-Time" element
                 
              })
              .ToListAsync();


            if (categorys.Count() == 0)
            {
                data = new GetAllMachineInformationAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,

                };

            }
            else
            {
                var lastRunTimes = categorys.Select(x => Convert.ToDecimal(x.LastRunTime?.Value)).Skip(1).ToList();

                data =
                new GetAllMachineInformationAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = DateTime.Now,
                    ValueRunning = lastRunTimes.FirstOrDefault(),
                   
                };
            }

            return await Result<GetAllMachineInformationAssyWheelLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
