using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation
{
    public record GetAllMachineInformationQuery : IRequest<Result<GetAllMachineInformationDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllMachineInformationQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
        internal class GetAllMachineInformationHandler : IRequestHandler<GetAllMachineInformationQuery, Result<GetAllMachineInformationDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllMachineInformationHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Result<GetAllMachineInformationDto>> Handle(GetAllMachineInformationQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.MachineId == m.MachineId && m.Subject.Vid.Contains("CYCLE-COUNT")) 
                || (query.MachineId == m.MachineId && m.Subject.Vid.Contains("RUN-TIME")) 
                || (query.MachineId == m.MachineId && m.Subject.Vid.Contains("RIM"))).ToListAsync();


                IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
                string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
                string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

                var data = new GetAllMachineInformationDto();

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
                     LastKalibrasi = groups.Where(g => g.Id.Contains("RIM"))
                         .OrderByDescending(g => g.DateTime)
                         .FirstOrDefault(), // Get the last "rim-calibration" element
                 })
                 .ToListAsync();


            if (categorys.Count() == 0)
            {
                data = new GetAllMachineInformationDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                   
                };

            }
            else
            {


                data = new GetAllMachineInformationDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = DateTime.Now,
                    ValueRunning = categorys.Select(c => Convert.ToDecimal(c.LastCycleCount?.Value)).FirstOrDefault(),
                    CycleCount = categorys.Select(x => Convert.ToDecimal(x.LastRunTime?.Value)).Skip(1).FirstOrDefault(),
                    LastTimeCalibration = categorys.Select(n => n.LastKalibrasi?.Value).Skip(2).FirstOrDefault(),

                };
            }

            return await Result<GetAllMachineInformationDto>.SuccessAsync(data, "Successfully fetch data");
            }

        }

    }


