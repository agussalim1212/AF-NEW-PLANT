using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
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

        public string search_term { get; set; }

        public GetListQualityGensubQuery() { }

        public GetListQualityGensubQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
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
        private readonly IDapperReadDbConnection _dapperReadDbConnection;

        public GetListQualityWithPaginationQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dapperReadDbConnection = dapperReadDbConnection;
        }

        public async Task<PaginatedResult<GetListQualityGensubDto>> Handle(GetListQualityGensubQuery query, CancellationToken cancellationToken)
        {
            var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (query.machine_id == m.MachineId && m.Subject.Vid.Contains("STATUS-PRDCT"))).ToListAsync();

            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();

            List<GetListQualityGensubDto> dt = new List<GetListQualityGensubDto>();
            var data = new GetListQualityGensubDto();

            var statusConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                   (@"SELECT * FROM ""list_quality_gensub"" WHERE id = ANY(@vid)
                       AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                       ORDER BY  bucket DESC",
                   new { vid = vids.ToList(), dateNow = DateTime.Now.Date, });

            if (statusConsumption.Count() == 0)
            {
                data =
                new GetListQualityGensubDto
                {
                    DateTime = DateTime.Now,
                    Status = "-",
                };
            }
            else
            {

                foreach (var s in statusConsumption)
                {
                    GetListQualityGensubDto listQuality = new GetListQualityGensubDto();

                    var status = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                    if (status != null && status.Value.Contains("1"))
                    {
                        listQuality.Status = "OK";
                    }
                    else
                    {
                        listQuality.Status = "NG";
                    }
                    listQuality.DateTime = s.Bucket.AddHours(7);
                    dt.Add(listQuality);

                }
            }

            var paginatedList = dt.Where(c => query.search_term == null
            || query.search_term.ToLower() == c.Status.ToLower())
           .Skip((query.page_number - 1) * query.page_size)
           .Take(query.page_size)
           .ToList();

            return new PaginatedResult<GetListQualityGensubDto>(paginatedList);
        }
    }

}
    
