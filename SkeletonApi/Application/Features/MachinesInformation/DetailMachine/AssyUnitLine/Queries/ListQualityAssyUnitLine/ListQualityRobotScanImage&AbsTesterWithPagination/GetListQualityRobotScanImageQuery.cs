using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage
{
   public record GetListQualityRobotScanImageQuery : IRequest<PaginatedResult<GetListQualityRobotScanImageDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string? search_term { get; set; }

        public GetListQualityRobotScanImageQuery() { }

        public GetListQualityRobotScanImageQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }

        internal class GetListQualityRobotScanImageQueryHandler : IRequestHandler<GetListQualityRobotScanImageQuery, PaginatedResult<GetListQualityRobotScanImageDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityRobotScanImageQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityRobotScanImageDto>> Handle(GetListQualityRobotScanImageQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();

                List<GetListQualityRobotScanImageDto> dt = new List<GetListQualityRobotScanImageDto>();
                var data = new GetListQualityRobotScanImageDto();

                var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
                var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();


                var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                        (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                var statusConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                       (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                       AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                       ORDER BY  bucket DESC",
                       new { vid = status.Subject.Vid, dateNow = DateTime.Now.Date, });

                if (statusConsumption.Count() == 0)
                {
                    data =
                    new GetListQualityRobotScanImageDto
                    {
                        DateTime = DateTime.Now,
                        Status = "-",
                        DataBarcode = "-",
                      
                    };
                }
                else
                {

                    foreach (var s in statusConsumption)
                    {
                        GetListQualityRobotScanImageDto listQuality = new GetListQualityRobotScanImageDto();

                        var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                        if (Barcode != null)
                        {
                            listQuality.DataBarcode = Barcode.Value;
                        }

                        var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                        if (statuss != null && statuss.Value.Contains("1"))
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

                var paginatedList = dt.Where(c => query.search_term == null || query.search_term.ToLower() == c.DataBarcode.ToLower()
                || query.search_term.ToLower() == c.Status.ToLower())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
