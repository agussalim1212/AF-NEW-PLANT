using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine
{
      public record GetListQualityMainLineQuery : IRequest<PaginatedResult<GetListQualityMainLineDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string? search_term { get; set; }

        public GetListQualityMainLineQuery() { }

        public GetListQualityMainLineQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }

        internal class GetListQualityMainLineQueryHandler : IRequestHandler<GetListQualityMainLineQuery, PaginatedResult<GetListQualityMainLineDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityMainLineQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityMainLineDto>> Handle(GetListQualityMainLineQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();

                //IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();

                List<GetListQualityMainLineDto> dt = new List<GetListQualityMainLineDto>();
                var data = new GetListQualityMainLineDto();

                var v = machine.Where(m => m.Subject.Vid.Contains("TIME-OPARATION")).FirstOrDefault();
                var x = machine.Where(m => m.Subject.Vid.Contains("FRQ_INVERT")).FirstOrDefault();

                var inverterConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                         (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                         new { vid = v.Subject.Vid, now = DateTime.Now.Date });

                var frQConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                       (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                       new { vid = x.Subject.Vid, now = DateTime.Now.Date });

              

                if (inverterConsumption.Count() == 0)
                {
                    data =
                    new GetListQualityMainLineDto
                    {
                        DateTime = DateTime.Now,
                        FrqInverter = 0,
                        DurationStop = 0,
                       
                    };
                }
                else
                {

                    foreach (var f in inverterConsumption)
                    {
                        GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                        var frQ = frQConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                        if (frQ != null)
                        {
                            listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                        }
                        var inverter = inverterConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                        if (inverter != null)
                        {
                            listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                        }
                        listQuality.DateTime = f.Bucket.AddHours(7);
                        dt.Add(listQuality);

                    }
                }

                var paginatedList = dt.Where(c => query.search_term == null || query.search_term == c.FrqInverter.ToString()
                || query.search_term == c.DurationStop.ToString())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
