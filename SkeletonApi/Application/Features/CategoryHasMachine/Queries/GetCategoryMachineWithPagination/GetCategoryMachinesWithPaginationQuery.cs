using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachineWithPagination;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachineWithPagination_;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachinesWithPagination
{
    public record GetCategoryMachinesWithPaginationQuery : IRequest<PaginatedResult<GetCategoryMachinesWithPaginationDto>>
    {
        //[JsonPropertyName("page_number")] tidak fungsi
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }


        public GetCategoryMachinesWithPaginationQuery() { }

        public GetCategoryMachinesWithPaginationQuery(int pageNumber, int pageSize, string SearchTerm)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = SearchTerm;


        }
    }
    internal class GetCategoryMachinesWithPaginationQueryHandler : IRequestHandler<GetCategoryMachinesWithPaginationQuery, PaginatedResult<GetCategoryMachinesWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryMachinesWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetCategoryMachinesWithPaginationDto>> Handle(GetCategoryMachinesWithPaginationQuery query, CancellationToken cancellationToken)
        {
           return await _unitOfWork.Repo<CategoryMachineHasMachine>().Entities.Where(x => query.search_term == null || x.CategoryMachine.Name.ToLower() == query.search_term.ToLower().Trim())
                .Include(o => o.Machine)
                .Include(x => x.CategoryMachine).GroupBy(x => new { x.CategoryMachineId, x.CategoryMachine.Name, x.CategoryMachine.UpdatedAt })
                .Select(g => new GetCategoryMachinesWithPaginationDto
                {
                    CategoryMachineId = g.Key.CategoryMachineId,
                    CategoryName = g.Key.Name,
                    UpdatedAt = g.Key.UpdatedAt.Value.AddHours(7),
                    Machine = g.Select(s => new MachineDto
                    {
                        Id = s.Machine.Id,
                        Name = s.Machine.Name,
                    }).ToList(),
                }).ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}
