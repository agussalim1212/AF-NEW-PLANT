using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Dashboard.FiveTopEnergyConsumption.Queries
{
    public record GetAllTop5EnergyConsumptionsQuery : IRequest<Result<GetAllTop5EnergyConsumptionsDto>>;
    internal class GetAllTop5EnergyConsumptionsQueryHandler : IRequestHandler<GetAllTop5EnergyConsumptionsQuery, Result<GetAllTop5EnergyConsumptionsDto>>
    {
        private readonly IDashboardRepository _dashboardRepository;

        public GetAllTop5EnergyConsumptionsQueryHandler(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }


        public async Task<Result<GetAllTop5EnergyConsumptionsDto>> Handle(GetAllTop5EnergyConsumptionsQuery query, CancellationToken cancellationToken)
        {
            var data = await _dashboardRepository.GetAllTop5EnergyConsumptionsAsync();
            return await Result<GetAllTop5EnergyConsumptionsDto>.SuccessAsync(data, "Successfully fetch data");
        }


    }
}
