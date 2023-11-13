using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.StopLine
{
    public record GetAllStopLineQuery : IRequest<Result<GetAllStopLineDto>>
    {   
        public Guid MachineId { get; set; }
        public GetAllStopLineQuery(Guid machineId) 
        { 
            MachineId = machineId;
        }
    }

    internal class GetAllStopLineHandler : IRequestHandler<GetAllStopLineQuery, Result<GetAllStopLineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllStopLineHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllStopLineDto>> Handle(GetAllStopLineQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("OFF-TIME")).ToListAsync();
            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllStopLineDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities.Where(c => vids.Contains(c.Id)).Select(g =>
                new
                {
                    Id = g.Id,
                    DateTime = g.DateTime,
                    Value = g.Id.Contains("OFF-TIME") ? Convert.ToInt32(g.Value) : 0,

                }).GroupBy(c => c.Id).Select(o => new
                {
                    StopTime = (o.Sum(x => x.Value)),
                    TotalStop = o.Count(),
                    
                }).ToListAsync();

            if (categorys.Count() == 0)
            {
                data = 
                        new GetAllStopLineDto
                        {
                            MachineName = machineName,
                            SubjectName = subjectName,
                        };

            }
            else
            {

               
            data =
                    new GetAllStopLineDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        DateTime = DateTime.Now,
                        TotalStop = categorys.Select(x => x.TotalStop).FirstOrDefault(),
                        StopTime = categorys.Select(c => c.StopTime).FirstOrDefault(),

                    };
            }

            return await Result<GetAllStopLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }


}
