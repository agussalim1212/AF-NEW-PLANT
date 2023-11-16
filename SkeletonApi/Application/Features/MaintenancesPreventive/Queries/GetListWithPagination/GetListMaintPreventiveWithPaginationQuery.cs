﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetDetail;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetListWithPagination
{
    public record GetListMaintPreventiveWithPaginationQuery : IRequest<PaginatedResult<GetListMaintPreventiveWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public Guid? machine_id { get; set; }
        public string? search_term { get; set; }

        public GetListMaintPreventiveWithPaginationQuery()
        {
            
        }

        public GetListMaintPreventiveWithPaginationQuery(int pageNumber, int pageSize, Guid? machineid, string? searchTerm)
        {
            machine_id = machineid;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }

    internal class GetListMaintPreventiveWithPaginationQueryHandle : IRequestHandler<GetListMaintPreventiveWithPaginationQuery, PaginatedResult<GetListMaintPreventiveWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListMaintPreventiveWithPaginationQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetListMaintPreventiveWithPaginationDto>> Handle(GetListMaintPreventiveWithPaginationQuery query, CancellationToken cancellationToken)
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
                        (preventive, machine) => new GetListMaintPreventiveWithPaginationDto
                        {
                            Id = preventive.Id,
                            MachineId = preventive.MachineId,
                            Name = machine.Name,
                            Plan = preventive.Plan,
                            StartDate = preventive.StartDate,
                            Actual = preventive.Actual,
                            EndDate = preventive.EndDate,
                            ok = preventive.EndDate == null ? false : true,
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
                            (preventive, machine) => new GetListMaintPreventiveWithPaginationDto
                            {
                                Id = preventive.Id,
                                MachineId = preventive.MachineId,
                                Name = machine.Name,
                                Plan = preventive.Plan,
                                StartDate = preventive.StartDate,
                                Actual = preventive.Actual,
                                EndDate = preventive.EndDate,
                                ok = preventive.EndDate == null ? false : true,
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
                        (preventive, machine) => new GetListMaintPreventiveWithPaginationDto
                        {
                            Id = preventive.Id,
                            MachineId = preventive.MachineId,
                            Name = machine.Name,
                            Plan = preventive.Plan,
                            StartDate = preventive.StartDate,
                            Actual = preventive.Actual,
                            EndDate = preventive.EndDate,
                            ok = preventive.EndDate == null ? false : true,
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
                            (preventive, machine) => new GetListMaintPreventiveWithPaginationDto
                            {
                                Id = preventive.Id,
                                MachineId = preventive.MachineId,
                                Name = machine.Name,
                                Plan = preventive.Plan,
                                StartDate = preventive.StartDate,
                                Actual = preventive.Actual,
                                EndDate = preventive.EndDate,
                                ok = preventive.EndDate == null ? false : true,
                            }
                        )
                        .OrderByDescending(x => x.StartDate)
                        .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
                }
            }
        }
    }
}
