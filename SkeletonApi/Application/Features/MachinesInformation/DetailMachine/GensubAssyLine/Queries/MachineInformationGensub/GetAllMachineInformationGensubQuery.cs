using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.MachineInformation
{
     public record GetAllMachineInformationGensubQuery : IRequest<Result<GetAllMachineInformationGensubDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationGensubQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetAllMachineInformationGensubHandler : IRequestHandler<GetAllMachineInformationGensubQuery, Result<GetAllMachineInformationGensubDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMachineInformationGensubHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllMachineInformationGensubDto>> Handle(GetAllMachineInformationGensubQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => (query.MachineId == m.MachineId 
            && m.Subject.Vid.Contains("CYCLE-COUNT")) || (query.MachineId == m.MachineId && m.Subject.Vid.Contains("RUN-TIME"))).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllMachineInformationGensubDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities
              .Where(c => vids.Contains(c.Id))
              .GroupBy(c => c.Id)
              .Select(groups => new
              {
                  Id = groups.Key, // ID dari kelompok
                  LastRunTime = groups.Where(g => g.Id.Contains("RUN-TIME"))
                      .OrderByDescending(g => g.DateTime)
                      .FirstOrDefault(), // Get the last "Run-Time" element
                  LastCycleCount = groups.Where(g => g.Id.Contains("CYCLE-COUNT"))
                      .OrderByDescending(g => g.DateTime)
                      .FirstOrDefault(), // Get the last "Cycle-Count" element
              })
              .ToListAsync();


            if (categorys.Count() == 0)
            {
                data = new GetAllMachineInformationGensubDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,

                };
            }
            else
            {
                var lastRunTimes = categorys.Select(x => Convert.ToDecimal(x.LastRunTime?.Value)).Skip(1).ToList();
                var lastCycleCounts = categorys.Select(c => Convert.ToDecimal(c.LastCycleCount?.Value)).ToList();

                data =
                new GetAllMachineInformationGensubDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = DateTime.Now,
                    ValueRunning = lastRunTimes.FirstOrDefault(),
                    CycleCount = lastCycleCounts.FirstOrDefault(),

                };
            }
            return await Result<GetAllMachineInformationGensubDto>.SuccessAsync(data, "Successfully fetch data");
        }
    }
}
