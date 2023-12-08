using AutoMapper;
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
        public string search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }

        public GetListQualityGensubQuery() { }

        public GetListQualityGensubQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string Type, DateTime Start, DateTime End)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = Type;
            start = Start;
            end = End;
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
        
            var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
            (@"SELECT * FROM ""list_quality_gensub"" WHERE id = ANY(@vid)
            AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
            ORDER BY  bucket DESC",
            new { vid = vids.ToList(), dateNow = DateTime.Now.Date, });

            switch (query.type)
            {
                case "day":
                    if (query.end.Date < query.start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_gensub"" WHERE id = ANY(@vid)
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = query.start.Date, endtime = query.end.Date });

                        if (consumptionBucket.Count() == 0)
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

                            foreach (var s in consumptionBucket)
                            {
                                GetListQualityGensubDto listQuality = new GetListQualityGensubDto();

                                if (s != null && s.Value.Contains("1"))
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
                    }
                    break;
                case "month":
                    if (query.end.Date < query.start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var consumptionBucket = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_gensub"" WHERE id = ANY(@vid)
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = vids.ToList(), starttime = query.start.Date, endtime = query.end.Date });

                        if (consumptionBucket.Count() == 0)
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

                            foreach (var s in consumptionBucket)
                            {
                                GetListQualityGensubDto listQuality = new GetListQualityGensubDto();

                                if (s != null && s.Value.Contains("1"))
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
                    }
                    break;
                default:
                var statussConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
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
                break;

            }

            var paginatedList = dt.Where(c => query.search_term == null
            || (query.search_term.ToLower() == c.Status.ToLower())).ToList();

            return await dt.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }

}
    
