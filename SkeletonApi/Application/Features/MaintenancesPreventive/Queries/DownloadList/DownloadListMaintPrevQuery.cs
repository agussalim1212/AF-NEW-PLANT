using AutoMapper;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.DownloadList
{
    public record DownloadListMaintPrevQuery : IRequest<PaginatedResult<DownloadListMaintPrevDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public Guid? machine_id { get; set; }
        public string? search_term { get; set; }

        public DownloadListMaintPrevQuery(){}

        public DownloadListMaintPrevQuery(int pageNumber, int pageSize, Guid? machineid, string? searchTerm)
        {
            machine_id = machineid;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }

    internal class DownloadListMaintPrevQueryHandler : IRequestHandler<DownloadListMaintPrevQuery, PaginatedResult<DownloadListMaintPrevDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DownloadListMaintPrevQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<DownloadListMaintPrevDto>> Handle(DownloadListMaintPrevQuery query, CancellationToken cancellationToken)
        {
            if (query.machine_id == null || string.IsNullOrWhiteSpace(query.machine_id.ToString().Trim()))
            {
                if (string.IsNullOrWhiteSpace(query.search_term) == true || query.search_term == null)
                {

                    return await _unitOfWork.Repository<MaintenacePreventive>().Entities
                    .Join(
                        _unitOfWork.Repository<Machine>().Entities,
                        preventive => preventive.MachineId,
                        machine => machine.Id,
                        (preventive, machine) => new DownloadListMaintPrevDto
                        {
                            Name = machine.Name,
                            Plan = preventive.Plan,
                            StartDate = preventive.StartDate,
                            Actual = preventive.Actual,
                            EndDate = preventive.EndDate,
                            Status_OK = preventive.EndDate == null ? "false" : "true",
                        }
                    )
                    .OrderByDescending(x => x.StartDate)
                    .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
                else
                {
                    return await _unitOfWork.Repository<MaintenacePreventive>().Entities
                        .Where(x =>
                        x.Plan.ToLower().Contains(query.search_term.ToLower().Trim()) ||
                        x.Actual.ToLower().Contains(query.search_term.ToLower().Trim()
                        ))
                        .Join(
                            _unitOfWork.Repository<Machine>().Entities,
                            preventive => preventive.MachineId,
                            machine => machine.Id,
                            (preventive, machine) => new DownloadListMaintPrevDto
                            {
                                Name = machine.Name,
                                Plan = preventive.Plan,
                                StartDate = preventive.StartDate,
                                Actual = preventive.Actual,
                                EndDate = preventive.EndDate,
                                Status_OK = preventive.EndDate == null ? "false" : "true",
                            }
                        )
                        .OrderByDescending(x => x.StartDate)
                        .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(query.search_term) == true || query.search_term == null)
                {

                    return await _unitOfWork.Repository<MaintenacePreventive>().Entities
                        .Where(x => x.MachineId.Equals(query.machine_id))
                    .Join(
                        _unitOfWork.Repository<Machine>().Entities,
                        preventive => preventive.MachineId,
                        machine => machine.Id,
                        (preventive, machine) => new DownloadListMaintPrevDto
                        {
                            Name = machine.Name,
                            Plan = preventive.Plan,
                            StartDate = preventive.StartDate,
                            Actual = preventive.Actual,
                            EndDate = preventive.EndDate,
                            Status_OK = preventive.EndDate == null ? "false" : "true",
                        }
                    )
                    .OrderByDescending(x => x.StartDate)
                    .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
                else
                {
                    return await _unitOfWork.Repository<MaintenacePreventive>().Entities
                        .Where(x => x.MachineId.Equals(query.machine_id) &&
                        x.Plan.ToLower().Contains(query.search_term.ToLower().Trim()) ||
                        x.Actual.ToLower().Contains(query.search_term.ToLower().Trim()
                        ))
                        .Join(
                            _unitOfWork.Repository<Machine>().Entities,
                            preventive => preventive.MachineId,
                            machine => machine.Id,
                            (preventive, machine) => new DownloadListMaintPrevDto
                            {
                                Name = machine.Name,
                                Plan = preventive.Plan,
                                StartDate = preventive.StartDate,
                                Actual = preventive.Actual,
                                EndDate = preventive.EndDate,
                                Status_OK = preventive.EndDate == null ? "false" : "true",
                            }
                        )
                        .OrderByDescending(x => x.StartDate)
                        .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
            }
        }
    }
}
