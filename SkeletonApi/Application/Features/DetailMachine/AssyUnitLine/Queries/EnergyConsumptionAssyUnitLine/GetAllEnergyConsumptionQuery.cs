using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetAllCategoryMachine;
using SkeletonApi.Application.Features.Dummys.DummyDto;
using SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectMachineWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine
{
    public record GetAllEnergyConsumptionQuery : IRequest<Result<GetAllEnergyConsumptionDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllEnergyConsumptionHandler : IRequestHandler<GetAllEnergyConsumptionQuery, Result<GetAllEnergyConsumptionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllEnergyConsumptionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<GetAllEnergyConsumptionDto>> Handle(GetAllEnergyConsumptionQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId).FirstOrDefaultAsync();

            var data = new GetAllEnergyConsumptionDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities.Where(c => machine.Subject.Vid.Contains("PWM-KWH")).Select(g =>
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
                new GetAllEnergyConsumptionDto
                {
                    MachineName = machine.Machine.Name,
                    SubjectName = machine.Subject.Subjects,
                })
                .ProjectTo<GetAllEnergyConsumptionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            }
            else
            {
                var category = await _unitOfWork.Data<Dummy>().Entities.Where(c => machine.Subject.Vid == c.Id).Select(g =>
                    new GetAllEnergyConsumptionDto
                    {
                        MachineName = machine.Machine.Name,
                        SubjectName = machine.Subject.Subjects,
                        Data = categorys.Select(val => new EnergyDto
                        {
                            ValueKwh = Convert.ToDecimal(val.Value),
                            ValueCo2 = Math.Round((Convert.ToDecimal(val.Value) * Convert.ToDecimal(0.87)),2),
                            Label = val.DateTime.AddHours(7).ToString("HH:mm:ss"),
                            DateTime = val.DateTime,
                            
                        }).ToList()
                    })
                    .ProjectTo<GetAllEnergyConsumptionDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                data = category;
            }
       
            return await Result<GetAllEnergyConsumptionDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}

