using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityGensubWithPagination
{
    public record GetListQualityGensubQuery : IRequest<PaginatedResult<GetListQualityGensubDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public DateTime? search_term { get; set; }

        public GetListQualityGensubQuery() { }

        public GetListQualityGensubQuery(DateTime searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }
    }
    internal class GetListQualityWithPaginationQueryHandler : IRequestHandler<GetListQualityGensubQuery, PaginatedResult<GetListQualityGensubDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListQualityWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetListQualityGensubDto>> Handle(GetListQualityGensubQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (query.machine_id == m.MachineId && m.Subject.Vid.Contains("COUNT-PRDCT-OK")) 
            || (query.machine_id == m.MachineId && m.Subject.Vid.Contains("COUNT-PRDCT-NG"))).ToListAsync();

            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();

           
            return await _unitOfWork.Data<Dummy>().Entities.Where(d => (vids.Contains(d.Id) && query.search_term == null) || (vids.Contains(d.Id) && d.DateTime == query.search_term)).Select(g => new GetListQualityGensubDto
            {
             Status = g.Id.Contains("COUNT-PRDCT-OK") ? "OK" : "NG",
             DateTime = g.DateTime.AddHours(7) 
            }).OrderByDescending(j => j.DateTime)
           .ProjectTo<GetListQualityGensubDto>(_mapper.ConfigurationProvider)
           .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);

        }
    }
}
