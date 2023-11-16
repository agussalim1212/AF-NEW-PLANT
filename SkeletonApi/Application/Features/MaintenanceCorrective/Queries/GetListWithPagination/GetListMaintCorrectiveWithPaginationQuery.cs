using AutoMapper;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetListWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetListWithPagination
{
    public record GetListMaintCorrectiveWithPaginationQuery : IRequest<PaginatedResult<GetListMaintCorrectiveWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public Guid? machine_id { get; set; }
        public string? search_term { get; set; }

        public GetListMaintCorrectiveWithPaginationQuery()
        {

        }

        public GetListMaintCorrectiveWithPaginationQuery(int pageNumber, int pageSize, Guid? machineid, string? searchTerm)
        {
            machine_id = machineid;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }

    internal class GetListMaintCorrectiveWithPaginationQueryHandle : IRequestHandler<GetListMaintCorrectiveWithPaginationQuery, PaginatedResult<GetListMaintCorrectiveWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListMaintCorrectiveWithPaginationQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetListMaintCorrectiveWithPaginationDto>> Handle(GetListMaintCorrectiveWithPaginationQuery query, CancellationToken cancellationToken)
        {
            if (query.machine_id == null || string.IsNullOrWhiteSpace(query.machine_id.ToString().Trim()))
            {
                if (string.IsNullOrWhiteSpace(query.search_term) == true || query.search_term == null)
                {

                    return await _unitOfWork.Repository<MaintCorrective>().Entities
                    .Join(
                        _unitOfWork.Repository<Machine>().Entities,
                        preventive => preventive.MachineId,
                        machine => machine.Id,
                        (preventive, machine) => new GetListMaintCorrectiveWithPaginationDto
                        {
                            Id = preventive.Id,
                            MachineId = preventive.MachineId,
                            Name = machine.Name,
                            StartDate = preventive.StartDate,
                            Actual = preventive.Actual,
                            EndDate = preventive.EndDate,
                        }
                    )
                    .OrderByDescending(x => x.StartDate)
                    .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
                else
                {
                    return await _unitOfWork.Repository<MaintCorrective>().Entities
                        .Where(x =>
                        x.Actual.ToLower().Contains(query.search_term.ToLower().Trim()
                        ))
                        .Join(
                            _unitOfWork.Repository<Machine>().Entities,
                            preventive => preventive.MachineId,
                            machine => machine.Id,
                            (preventive, machine) => new GetListMaintCorrectiveWithPaginationDto
                            {
                                Id = preventive.Id,
                                MachineId = preventive.MachineId,
                                Name = machine.Name,
                                StartDate = preventive.StartDate,
                                Actual = preventive.Actual,
                                EndDate = preventive.EndDate,
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

                    return await _unitOfWork.Repository<MaintCorrective>().Entities
                        .Where(x => x.MachineId.Equals(query.machine_id))
                    .Join(
                        _unitOfWork.Repository<Machine>().Entities,
                        preventive => preventive.MachineId,
                        machine => machine.Id,
                        (preventive, machine) => new GetListMaintCorrectiveWithPaginationDto
                        {
                            Id = preventive.Id,
                            MachineId = preventive.MachineId,
                            Name = machine.Name,
                            StartDate = preventive.StartDate,
                            Actual = preventive.Actual,
                            EndDate = preventive.EndDate,
                        }
                    )
                    .OrderByDescending(x => x.StartDate)
                    .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
                else
                {
                    return await _unitOfWork.Repository<MaintCorrective>().Entities
                        .Where(x => x.MachineId.Equals(query.machine_id) &&
                        x.Actual.ToLower().Contains(query.search_term.ToLower().Trim()
                        ))
                        .Join(
                            _unitOfWork.Repository<Machine>().Entities,
                            preventive => preventive.MachineId,
                            machine => machine.Id,
                            (preventive, machine) => new GetListMaintCorrectiveWithPaginationDto
                            {
                                Id = preventive.Id,
                                MachineId = preventive.MachineId,
                                Name = machine.Name,
                                StartDate = preventive.StartDate,
                                Actual = preventive.Actual,
                                EndDate = preventive.EndDate,
                            }
                        )
                        .OrderByDescending(x => x.StartDate)
                        .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
            }
        }
    }
}
