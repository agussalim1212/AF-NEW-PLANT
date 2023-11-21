using MediatR;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityNumbering
{
     public record GetListQualityNumberingQuery : IRequest<PaginatedResult<GetListQualityNumberingDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string? search_term { get; set; }

        public GetListQualityNumberingQuery() { }

        public GetListQualityNumberingQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }

        //internal class GetListQualityNumberingQueryHandler : IRequestHandler<GetListQualityNumberingQuery, PaginatedResult<GetListQualityNumberingDto>>
        //{
        //    private readonly IUnitOfWork _unitOfWork;
        //    private readonly IMapper _mapper;
        //    private readonly IDapperReadDbConnection _dapperReadDbConnection;

        //    public GetListQualityNumberingQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
        //    {
        //        _unitOfWork = unitOfWork;
        //        _mapper = mapper;
        //        _dapperReadDbConnection = dapperReadDbConnection;
        //    }

        //    public async Task<PaginatedResult<GetListQualityNumberingDto>> Handle(GetListQualityNumberingQuery query, CancellationToken cancellationToken)
        //    {
        //        var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
        //        .Where(m => (query.machine_id == m.MachineId)).ToListAsync();

        //        List<GetListQualityNumberingDto> dt = new List<GetListQualityNumberingDto>();
        //        var data = new GetListQualityNumberingDto();


        //        var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
        //        var scanImg = machine.Where(m => m.Subject.Vid.Contains("VOL-OIL-BRAEK")).FirstOrDefault();
        //        var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();

        //        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<NumberingConsumption>
        //                 (@"SELECT * FROM ""list_quality_nut_runner_steering_stem"" WHERE id = @vid
        //                 AND date_trunc('day', bucket::date) = date_trunc('day', @now)
        //                 ORDER BY  bucket DESC",
        //                 new { vid = "DCM_P9AEA0_MC9_ID-PART", now = DateTime.Now.Date });

        //        var scanImgConsumption = await _dapperReadDbConnection.QueryAsync<NumberingConsumption>
        //               (@"SELECT * FROM ""list_quality_nut_runner_steering_stem"" WHERE id = @vid
        //                AND date_trunc('day', bucket::date) = date_trunc('day', @now)
        //                ORDER BY  bucket DESC",
        //               new { vid = "DCM_P9AEA0_MC9_TORQ", now = DateTime.Now.Date });

        //        var statusConsumption = await _dapperReadDbConnection.QueryAsync<NumberingConsumption>
        //               (@"SELECT * FROM ""list_quality_nut_runner_steering_stem"" WHERE id = ANY(@vid)
        //               AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
        //               ORDER BY  bucket DESC",
        //               new { vid = vids.ToList(), dateNow = DateTime.Now.Date, });

        //        if (statusConsumption.Count() == 0)
        //        {
        //            data =
        //            new GetListQualityNumberingDto
        //            {
        //                DateTime = DateTime.Now,
        //                Status = "-",
        //                DataBarcode = "-",
        //                DataTorQ = 0
        //            };
        //        }
        //        else
        //        {

        //            foreach (var f in barcodeConsumption)
        //            {
        //                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

        //                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
        //                if (TorQ != null)
        //                {
        //                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
        //                }
        //                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
        //                if (Barcode != null)
        //                {
        //                    listQuality.DataBarcode = Barcode.Value;
        //                }

        //                var status = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
        //                if (status != null && status.Id.Contains("COUNT-PRDCT-OK"))
        //                {
        //                    listQuality.Status = "OK";
        //                }
        //                else
        //                {
        //                    listQuality.Status = "NG";
        //                }
        //                listQuality.DateTime = f.Bucket.AddHours(7);
        //                dt.Add(listQuality);

        //            }
        //        }


        //        var paginatedList = dt.Where(c => query.search_term == null || query.search_term.ToLower() == c.DataBarcode.ToLower()
        //        || query.search_term.ToLower() == c.Status.ToLower())
        //       .Skip((query.page_number - 1) * query.page_size)
        //       .Take(query.page_size)
        //       .ToList();

        //        return new PaginatedResult<GetListQualityNutRunnerSteeringStemDto>(paginatedList);
        //    }
        //}
    }
}
