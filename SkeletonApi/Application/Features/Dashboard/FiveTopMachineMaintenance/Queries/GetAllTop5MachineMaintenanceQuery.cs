using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Dashboard.FiveTopMachineMaintenance.Queries
{
    public record GetAllTop5MachineMaintenanceQuery : IRequest<Result<GetAllTop5MachineMaintenanceDto>>;

    internal class GetAllTop5MachineMaintenanceQueryHandler : IRequestHandler<GetAllTop5MachineMaintenanceQuery, Result<GetAllTop5MachineMaintenanceDto>>
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetAllTop5MachineMaintenanceQueryHandler(IDashboardRepository dashboardRepository, IUnitOfWork unitOfWork)
        {
            _dashboardRepository = dashboardRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetAllTop5MachineMaintenanceDto>> Handle(GetAllTop5MachineMaintenanceQuery query, CancellationToken cancellationToken)
        {
            var data = await _dashboardRepository.GetAllTop5MachineMaintenance();
            return await Result<GetAllTop5MachineMaintenanceDto>.SuccessAsync(data, "Successfully fetch data");
        }
    }
}