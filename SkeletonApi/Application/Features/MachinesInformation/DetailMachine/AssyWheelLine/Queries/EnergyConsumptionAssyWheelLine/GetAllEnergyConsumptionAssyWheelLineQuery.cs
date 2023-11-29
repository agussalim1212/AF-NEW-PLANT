﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine;
using SkeletonApi.Application.Features.Dummys.DummyDto;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine
{
    public record GetAllEnergyConsumptionAssyWheelLineQuery : IRequest<Result<GetAllEnergyConsumptionAssyWheelLineDto>>
    {
        public Guid MachineId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public GetAllEnergyConsumptionAssyWheelLineQuery(Guid machineId, string type, DateTime start, DateTime end)
        {
            MachineId = machineId;
            Type = type;
            Start = start;
            End = end;
        }
    }


    internal class GetAllEnergyConsumptionAssyWheelLineHandler : IRequestHandler<GetAllEnergyConsumptionAssyWheelLineQuery, Result<GetAllEnergyConsumptionAssyWheelLineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllEnergyConsumptionAssyWheelLineHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<GetAllEnergyConsumptionAssyWheelLineDto>> Handle(GetAllEnergyConsumptionAssyWheelLineQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject).Where(m => query.MachineId == m.MachineId && m.Subject.Vid.Contains("PWM-KWH")).ToListAsync();
            string vid = machine.Select(m => m.Subject.Vid).FirstOrDefault();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();
            var data = new GetAllEnergyConsumptionAssyWheelLineDto();

            var categorys = await _unitOfWork.Data<Dummy>().Entities.Where(c => vid == c.Id).Select(g =>
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
                data = new GetAllEnergyConsumptionAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    Data = new List<EnergyAssyDto>(),
                };
            }
            else
            {
                data =
                    new GetAllEnergyConsumptionAssyWheelLineDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        Data = categorys.Select(val => new EnergyAssyDto
                        {
                            ValueKwh = Convert.ToDecimal(val.Value),
                            ValueCo2 = Math.Round(Convert.ToDecimal(val.Value) * Convert.ToDecimal(0.87), 2),
                            Label = val.DateTime.AddHours(7).ToString("HH:mm:ss"),
                            DateTime = val.DateTime.AddHours(7),

                        }).ToList()
                    };

            }

            return await Result<GetAllEnergyConsumptionAssyWheelLineDto>.SuccessAsync(data, "Successfully fetch data");
        }

    }
}
