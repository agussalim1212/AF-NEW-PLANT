using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Dashboard._5_Top_Machine_Maintenance.Queries
{
    public record GetAllTop5MachineMaintenanceQuery : IRequest<Result<GetAllTop5MachineMaintenanceDto>>;
    internal class GetAllTop5MachineMaintenanceQueryHandler : IRequestHandler<GetAllTop5MachineMaintenanceQuery, Result<GetAllTop5MachineMaintenanceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDapperReadDbConnection _dapperReadDbConnection;


        public GetAllTop5MachineMaintenanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDapperReadDbConnection dapperReadDbConnection)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }

        public async Task<Result<GetAllTop5MachineMaintenanceDto>> Handle(GetAllTop5MachineMaintenanceQuery query, CancellationToken cancellationToken)
        {
            var querys = from shm in _unitOfWork.Repository<MaintenacePreventive>().Entities
                         join m in _unitOfWork.Repository<Machine>().Entities on shm.MachineId equals m.Id
                         select new { Machine = m.Name, MaintenanceId = shm.Id, Datetime = shm.CreatedAt };

           var result = await querys.ToListAsync();

           var data = new GetAllTop5MachineMaintenanceDto();

            if (result.Count() == 0)
            {
                data =
                new GetAllTop5MachineMaintenanceDto
                {
                    TotalWeek = 0,
                    TotalMonth = 0,
                    DataMaintenance = new List<DataMaintenance>(),
                };
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                data =
                new GetAllTop5MachineMaintenanceDto
                {
                    TotalWeek = querys.Where(d => d.Datetime >= now.AddDays(-7)).Select(d => d.MaintenanceId).Count(),
                    TotalMonth = querys.Where(g => g.Datetime >= now.AddMonths(-1)).Select(g => g.MaintenanceId).Count(),
                    DataMaintenance = querys.GroupBy(h => new { h.Machine })
                    .Select(val => new DataMaintenance
                    {
                        Label = querys.Where(k => val.Key.Machine == k.Machine).Select(v => v.Machine).FirstOrDefault(),
                        Value = val.Select(n => Convert.ToDecimal(n.MaintenanceId)).Count()
                    }).OrderByDescending(v => v.Value).Take(5).ToList()
                };
            }

           // TotalWeek = querys.Where(d => d.Datetime >= now.AddDays(-7)).Select(d => d.MaintenanceId).Count(),
            // TotalMonth = querys.Where(g => g.Datetime >= now.AddMonths(-1)).Select(g => g.MaintenanceId).Count(),

            return await Result<GetAllTop5MachineMaintenanceDto>.SuccessAsync(data, "Successfully fetch data");
        }
}

}
