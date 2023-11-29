using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions
{
    public record GetAllDetailEnergyConsumptionQuery : IRequest<Result<List<GetAllDetailEnergyConsumptionDto>>>
    {
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public GetAllDetailEnergyConsumptionQuery(string Type, DateTime Start, DateTime End)
        {
            type = Type;
            start = Start;
            end = End;
        }
    }
    internal class GetAllDetailEnergyConsumptionQueryHandler : IRequestHandler<GetAllDetailEnergyConsumptionQuery, Result<List<GetAllDetailEnergyConsumptionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;


        public GetAllDetailEnergyConsumptionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDapperReadDbConnection dapperReadDbConnection)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }


        public async Task<Result<List<GetAllDetailEnergyConsumptionDto>>> Handle(GetAllDetailEnergyConsumptionQuery query, CancellationToken cancellationToken)
        {
            var querys = from shm in _unitOfWork.Repo<SubjectHasMachine>().Entities
                         join s in _unitOfWork.Repository<Subject>().Entities on shm.SubjectId equals s.Id
                         join m in _unitOfWork.Repository<Machine>().Entities on shm.MachineId equals m.Id
                         where s.Vid.Contains("PWM-KWH")
                         select new { Machine = m, Subject = s, SubjectMachine = shm };

            var result = await querys.ToListAsync();

            List<GetAllDetailEnergyConsumptionDto> dt = new List<GetAllDetailEnergyConsumptionDto>();
            var data = new GetAllDetailEnergyConsumptionDto();

            var EnergyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                     (@"SELECT * FROM ""top_five_energy_consumption"" WHERE id = ANY(@vid)
                     AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                     ORDER BY  bucket DESC",
                     new { vid = querys.Select(o => o.Subject.Vid).ToList(), now = DateTime.Now.Date });


            if (EnergyConsumption.Count() == 0)
            {
                data =
                new GetAllDetailEnergyConsumptionDto
                {
                    ValueKwh = 0,
                    ValueCo2 = 0,
                    Label = "",
                    DateTime = DateTime.Now
                };
                dt.Add(data);
            }
            else
            {
                data =
                new GetAllDetailEnergyConsumptionDto
                {
                    ValueKwh = EnergyConsumption.Sum(g => Convert.ToDecimal(g.Value)),
                    ValueCo2 = EnergyConsumption.Sum(g => Math.Round((Convert.ToDecimal(g.Value) * Convert.ToDecimal(0.87)),2)),
                    Label = EnergyConsumption.Select(g => g.Bucket.ToString("dd-MM-yy")).FirstOrDefault(),
                    DateTime = DateTime.Now 
                   
                };
                dt.Add(data);
            } 

            return await Result<List<GetAllDetailEnergyConsumptionDto>>.SuccessAsync(dt);
        }


    }
}
