using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.FrameNumb.Queries.GetFrameNumberWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.FrameNumberSubject.Queries.GetFrameNumberHasSubjectWithPagination
{
    public record GetFrameNumberHasSubjectWithPaginationQuery : IRequest<PaginatedResult<GetFrameNumberHasSubjectWithPaginationDto>>
    {

        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }

        public GetFrameNumberHasSubjectWithPaginationQuery() { }

        public GetFrameNumberHasSubjectWithPaginationQuery(int pageNumber, int pageSize, string searchTerm)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;


        }
    }
    internal class GetSubjectMachinesWithPaginationQueryHandler : IRequestHandler<GetFrameNumberHasSubjectWithPaginationQuery, PaginatedResult<GetFrameNumberHasSubjectWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubjectMachinesWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetFrameNumberHasSubjectWithPaginationDto>> Handle(GetFrameNumberHasSubjectWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repo<FrameNumberHasSubjects>().FindByCondition(x => x.DeletedAt == null)
            .Where(s => query.search_term == null 
            || query.search_term.ToLower() == s.Subject.Subjects
            || query.search_term.ToLower() == s.FrameNumber.Name.ToLower())
            .OrderBy(x => x.UpdatedAt).Include(c => c.Subject).Include(n => n.FrameNumber)
            .GroupBy(k => new {k.FrameNumber.Id, k.FrameNumber.Name, k.FrameNumber.UpdatedAt})
            .Select(m => new GetFrameNumberHasSubjectWithPaginationDto
            {
                FrameNumberId = m.Key.Id,
                FrameNumberName = m.Key.Name,
                UpdateAt = m.Key.UpdatedAt.Value.AddHours(7),
                Data = m.Select(p => new SubjectsDto
                {
                    Id = p.Subject.Id,
                    SubjectName = p.Subject.Subjects

                }).ToList()
            })
            .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }


    }
}
