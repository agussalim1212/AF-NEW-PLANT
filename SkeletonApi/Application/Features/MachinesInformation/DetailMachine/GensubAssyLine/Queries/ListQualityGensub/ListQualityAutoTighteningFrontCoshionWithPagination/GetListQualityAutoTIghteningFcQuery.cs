using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityAutoTighteningFrontCoshionWithPagination
{
    public record GetListQualityAutoTIghteningFcQuery : IRequest<PaginatedResult<GetListQualityAutoTIghteningFcDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string? search_term { get; set; }

        public GetListQualityAutoTIghteningFcQuery() { }

        public GetListQualityAutoTIghteningFcQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }

        internal class GetListQualityAutoTIghteningFcQueryHandler : IRequestHandler<GetListQualityAutoTIghteningFcQuery, PaginatedResult<GetListQualityAutoTIghteningFcDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityAutoTIghteningFcQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityAutoTIghteningFcDto>> Handle(GetListQualityAutoTIghteningFcQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId && m.Subject.Vid.Contains("DCM_P9AEA0_MC21_STATUS-PRDCT"))).ToListAsync();

                IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();

                List<GetListQualityAutoTIghteningFcDto> dt = new List<GetListQualityAutoTIghteningFcDto>();
                var data = new GetListQualityAutoTIghteningFcDto();

                var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                        (@"SELECT * FROM ""list_quality_auto_tightening_front_cusion"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = "DCM_P9AEA0_MC21_ID-PART", now = DateTime.Now.Date });

                var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                       (@"SELECT * FROM ""list_quality_auto_tightening_front_cusion"" WHERE id = @vid
                       AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                       ORDER BY  bucket DESC",
                       new { vid = "DCM_P9AEA0_MC21_TORQ", now = DateTime.Now.Date });

                var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                       (@"SELECT * FROM ""list_quality_auto_tightening_front_cusion"" WHERE id = ANY(@vid)
                       AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                       ORDER BY  bucket DESC",
                       new { vid = vids.ToList(), dateNow = DateTime.Now.Date });

                if (statusConsumption.Count() == 0)
                {
                    data =
                    new GetListQualityAutoTIghteningFcDto
                    {
                        DateTime = DateTime.Now,
                        Status = "-",
                        DataBarcode = "-",
                        DataTorQ = 0
                    };
                }
                else
                {

                    foreach (var f in barcodeConsumption)
                    {
                        GetListQualityAutoTIghteningFcDto listQuality = new GetListQualityAutoTIghteningFcDto();

                        var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                        if (TorQ != null )
                        {
                            listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                        }
                        var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                        if (Barcode != null)
                        {
                            listQuality.DataBarcode = Barcode.Value;
                        }

                        var status = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                        if (status != null && status.Value.Contains("1"))
                        {
                            listQuality.Status = "OK";
                        }
                        else
                        {
                            listQuality.Status = "NG";
                        }
                        listQuality.DateTime = f.Bucket.AddHours(7);
                        dt.Add(listQuality);

                    }
                }

                var paginatedList = dt.Where(c => query.search_term == null || query.search_term.ToLower() == c.DataBarcode.ToLower()
                || query.search_term.ToLower() == c.Status.ToLower())
               .Skip((query.page_number - 1) * query.page_size)
               .Take(query.page_size)
               .ToList();

                return new PaginatedResult<GetListQualityAutoTIghteningFcDto>(paginatedList);
            }
        }
    }

}
