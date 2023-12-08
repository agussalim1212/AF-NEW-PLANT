using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.FrequencyInverter
{
   public record GetAllFrequencyInverterQuery : IRequest<Result<GetAllFrequencyInverterDto>>
    {
        public Guid MachineId { get; set; }

        public GetAllFrequencyInverterQuery(Guid machineId)
        {
            MachineId = machineId;
        }

    }
    internal class GetAllMachineInformationHandler : IRequestHandler<GetAllFrequencyInverterQuery, Result<GetAllFrequencyInverterDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllMachineInformationHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllFrequencyInverterDto>> Handle(GetAllFrequencyInverterQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("FRQ_INVERT")).ToListAsync();
            string vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllFrequencyInverterDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities
              .Where(c => vid.Contains(c.Id)).OrderByDescending(c => c.DateTime).Take(1)
              .ToListAsync();

            if (categorys.Count() == 0)
            {
                data = new GetAllFrequencyInverterDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,

                };

            }
            else
            {
               
                data = new GetAllFrequencyInverterDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = categorys.Select(x => x.DateTime.AddHours(7)).FirstOrDefault(),
                    Value = categorys.Select(x => Convert.ToDecimal(x.Value)).FirstOrDefault()

                };
            }

            return await Result<GetAllFrequencyInverterDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
