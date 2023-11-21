using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachinesWithPagination;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachineWithPagination;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectMachineWithPagination
{
    public record GetSubjectMachineWithPaginationQuery : IRequest<PaginatedResult<GetSubjectMachineWithPaginationDto>>
    {
        
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetSubjectMachineWithPaginationQuery() { }

        public GetSubjectMachineWithPaginationQuery(int pageNumber, int pageSize, string searchTerm)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;


        }
    }
    internal class GetSubjectMachinesWithPaginationQueryHandler : IRequestHandler<GetSubjectMachineWithPaginationQuery, PaginatedResult<GetSubjectMachineWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubjectMachinesWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetSubjectMachineWithPaginationDto>> Handle(GetSubjectMachineWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repo<SubjectHasMachine>().Entities.Where(x => query.search_term == null || x.Machine.Name.ToLower() == query.search_term.ToLower().Trim())
                 .Include(o => o.Subject)
                 .Include(x => x.Machine).GroupBy(x => new { x.Machine.Id, x.Machine.Name, x.Machine.UpdatedAt})
                 .Select(g => new GetSubjectMachineWithPaginationDto
                 {
                     MachineId = g.Key.Id,
                     MachineName = g.Key.Name,
                     UpdatedAt = g.Key.UpdatedAt.Value.AddHours(7),
                     Subjects = g.Select(s => new SubjectDto
                     {
                         SubjectId = s.Subject.Id,
                         SubjectName = s.Subject.Subjects,
                     }).ToList(),
                 }).ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }


    }
}
