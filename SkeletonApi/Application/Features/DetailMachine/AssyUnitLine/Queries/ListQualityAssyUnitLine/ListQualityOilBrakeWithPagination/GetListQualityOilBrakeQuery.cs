using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrake
{
    public record GetListQualityOilBrakeQuery : IRequest<PaginatedResult<GetListQualityOilBrakeDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string? search_term { get; set; }

        public GetListQualityOilBrakeQuery() { }

        public GetListQualityOilBrakeQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }

        internal class GetListQualityOilBrakeQueryHandler : IRequestHandler<GetListQualityOilBrakeQuery, PaginatedResult<GetListQualityOilBrakeDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityOilBrakeQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityOilBrakeDto>> Handle(GetListQualityOilBrakeQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();


                List<GetListQualityOilBrakeDto> dt = new List<GetListQualityOilBrakeDto>();
                var data = new GetListQualityOilBrakeDto();

                var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
                var leak = machine.Where(m => m.Subject.Vid.Contains("LEAK-TES")).FirstOrDefault();
                var vol = machine.Where(m => m.Subject.Vid.Contains("VOL-OIL-BRAEK")).FirstOrDefault();
                var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
                var code = machine.Where(m => m.Subject.Vid.Contains("CODE")).FirstOrDefault();

                var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                         (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                         new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                var leakTesterConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                      (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                      new { vid = leak.Subject.Vid, now = DateTime.Now.Date });

                var volumeOilConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                      (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                      new { vid = vol.Subject.Vid, now = DateTime.Now.Date });

                var statusConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                       (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                       AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                       ORDER BY  bucket DESC",
                       new { vid = status.Subject.Vid, dateNow = DateTime.Now.Date, });

                var codeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                      (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                       AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                       ORDER BY  bucket DESC",
                   new { vid = code.Subject.Vid, dateNow = DateTime.Now.Date, });
 
                if (statusConsumption.Count() == 0)
                {
                    data =
                    new GetListQualityOilBrakeDto
                    {
                        DateTime = DateTime.Now,
                        DataBarcode = "-",
                        LeakTester = 0,
                        VolumeOilBrake = 0,
                        Status = "-",
                        ErrorCode = 0


                    };
                }
                else
                {

                    foreach (var s in statusConsumption)
                    {
                        GetListQualityOilBrakeDto listQuality = new GetListQualityOilBrakeDto();

                        var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                        if (Barcode != null)
                        {
                            listQuality.DataBarcode = Barcode.Value;
                        }
                        var leakTester = leakTesterConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                        if (leakTester != null)
                        {
                            listQuality.LeakTester =Convert.ToDecimal(leakTester.Value);
                        }
                        var volumeOil = volumeOilConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                        if (volumeOil != null)
                        {
                            listQuality.VolumeOilBrake = Convert.ToDecimal(volumeOil.Value);
                        }
                        var errorCode = codeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                        if (errorCode != null)
                        {
                            listQuality.ErrorCode = Convert.ToInt32(errorCode.Value);
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
