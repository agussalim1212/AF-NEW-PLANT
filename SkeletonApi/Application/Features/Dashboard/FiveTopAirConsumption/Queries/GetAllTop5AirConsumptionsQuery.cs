using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Dashboard.FiveTopAirConsumption.Queries
{
    public record GetAllTop5AirConsumptionsQuery : IRequest<Result<GetAllTop5AirConsumptionsDto>>;

    internal class GetAllTop5AirConsumptionsQueryHandler : IRequestHandler<GetAllTop5AirConsumptionsQuery, Result<GetAllTop5AirConsumptionsDto>>
    {
        private readonly IDashboardRepository _dashboardRepository;

        public GetAllTop5AirConsumptionsQueryHandler(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<Result<GetAllTop5AirConsumptionsDto>> Handle(GetAllTop5AirConsumptionsQuery query, CancellationToken cancellationToken)
        {
            var data = await _dashboardRepository.GetAllTop5AirConsumptionsAsync();
            return await Result<GetAllTop5AirConsumptionsDto>.SuccessAsync(data, "Successfully fetch data");
        }
    }
}