using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Dashboard._5_Top_Energy_Consumptions
{
    public record GetAllTop5EnergyConsumptionsQuery : IRequest<Result<GetAllTop5EnergyConsumptionsDto>>;
    internal class GetAllTop5EnergyConsumptionsQueryHandler : IRequestHandler<GetAllTop5EnergyConsumptionsQuery, Result<GetAllTop5EnergyConsumptionsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;


        public GetAllTop5EnergyConsumptionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDapperReadDbConnection dapperReadDbConnection)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }


        public async Task<Result<GetAllTop5EnergyConsumptionsDto>> Handle(GetAllTop5EnergyConsumptionsQuery query, CancellationToken cancellationToken)
        {
            var querys = from shm in _unitOfWork.Repo<SubjectHasMachine>().Entities
                         join s in _unitOfWork.Repository<Subject>().Entities on shm.SubjectId equals s.Id
                         join m in _unitOfWork.Repository<Machine>().Entities on shm.MachineId equals m.Id
                         where s.Vid.Contains("POWER-CONSUMPTION")
                         select new { Machine = m, Subject = s, SubjectMachine = shm };

            var result = await querys.ToListAsync();
            var data = new GetAllTop5EnergyConsumptionsDto();
            var EnergyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                     (@"SELECT * FROM ""top_five_energy_consumption"" WHERE id = ANY(@vid)
                     AND date_trunc('year', bucket::date) = date_trunc('year', @now)
                     ORDER BY  bucket DESC",
                     new { vid = querys.Select(o => o.Subject.Vid).ToList(), now = DateTime.Now.Date });

            var EnergyWeekConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                     (@"SELECT * FROM ""top_five_energy_consumption"" WHERE id = ANY(@vid)
                     AND date_trunc('year', bucket::date) = date_trunc('year', @now)
                     ORDER BY  bucket DESC",
                     new { vid = querys.Select(o => o.Subject.Vid).ToList(), now = DateTime.Now.Date });

            var EnergyMonthConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                     (@"SELECT * FROM ""top_five_energy_consumption"" WHERE id = ANY(@vid)
                     AND date_trunc('year', bucket::date) = date_trunc('year', @now)
                     ORDER BY  bucket DESC",
                     new { vid = querys.Select(o => o.Subject.Vid).ToList(), now = DateTime.Now.Date });

            if (EnergyConsumption.Count() == 0)
            {
                data = 
                new GetAllTop5EnergyConsumptionsDto
                {
                    TotalWeek = 0,
                    TotalMonth = 0,
                    DataMachines = new List<DataMachine>(),
                };
            }
            else
            {
                data =
                new GetAllTop5EnergyConsumptionsDto
                {
                    TotalWeek = EnergyWeekConsumption.Sum(g => Convert.ToDecimal(g.Value)),
                    TotalMonth = EnergyMonthConsumption.Sum(g => Convert.ToDecimal(g.Value)),
                    DataMachines = EnergyConsumption.GroupBy(h => new { h.Id })
                    .Select(val => new DataMachine
                    {
                        Label = querys.Where(k => val.Key.Id == k.Subject.Vid).Select(v => v.Machine.Name).FirstOrDefault(),
                        Value = val.Max(n => Convert.ToDecimal(n.Value))
                    }).OrderByDescending(v => v.Value).Take(5).ToList()
                };
            }

            return await Result<GetAllTop5EnergyConsumptionsDto>.SuccessAsync(data, "Successfully fetch data");
        }


    }
}
