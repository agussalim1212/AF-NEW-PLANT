using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling
{
    public record GetListQualityCoolantFilingQuery : IRequest<PaginatedResult<GetListQualityCoolantFilingDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public GetListQualityCoolantFilingQuery() { }

        public GetListQualityCoolantFilingQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string Type, DateTime Start, DateTime End)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = Type;
            start = Start;
            end = End;
        }

        internal class GetListQualityCoolantFilingQueryHandler : IRequestHandler<GetListQualityCoolantFilingQuery, PaginatedResult<GetListQualityCoolantFilingDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityCoolantFilingQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityCoolantFilingDto>> Handle(GetListQualityCoolantFilingQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();

                List<GetListQualityCoolantFilingDto> dt = new List<GetListQualityCoolantFilingDto>();
                var data = new GetListQualityCoolantFilingDto();

                var volumeVid = machine.Where(m => m.Subject.Vid.Contains("VOL-COLN")).FirstOrDefault();
                var barcodeVid = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();

                switch (query.type)
                {
                    case "day":

                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                           var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                           (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                           AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                           AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                           ORDER BY id DESC, bucket DESC", new { vid = volumeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                           var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                           (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                           AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                           AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                           ORDER BY id DESC, bucket DESC", new { vid = barcodeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (volumeConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityCoolantFilingDto
                                {
                                    DateTime = DateTime.Now,
                                    VolumeCoolant = 0,
                                    DataBarcode = "-",

                                };
                            }
                            else
                            {

                                foreach (var f in barcodeConsumption)
                                {
                                    GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                    var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (vol != null)
                                    {
                                        listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                    }
                                    var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                    if (barcode != null)
                                    {
                                        listQuality.DataBarcode = barcode.Value;

                                    }
                                    listQuality.DateTime = f.Bucket.AddHours(7);

                                    dt.Add(listQuality);

                                }
                            }
                        }
                            break;
                    case "week":

                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                            AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                            AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = volumeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                            AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                            AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = barcodeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (volumeConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityCoolantFilingDto
                                {
                                    DateTime = DateTime.Now,
                                    VolumeCoolant = 0,
                                    DataBarcode = "-",

                                };
                            }
                            else
                            {

                                foreach (var f in barcodeConsumption)
                                {
                                    GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                    var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (vol != null)
                                    {
                                        listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                    }
                                    var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                    if (barcode != null)
                                    {
                                        listQuality.DataBarcode = barcode.Value;

                                    }
                                    listQuality.DateTime = f.Bucket.AddHours(7);

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
                            var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                            AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                            AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = volumeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                            AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                            AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = barcodeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (volumeConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityCoolantFilingDto
                                {
                                    DateTime = DateTime.Now,
                                    VolumeCoolant = 0,
                                    DataBarcode = "-",

                                };
                            }
                            else
                            {

                                foreach (var f in barcodeConsumption)
                                {
                                    GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                    var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (vol != null)
                                    {
                                        listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                    }
                                    var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                    if (barcode != null)
                                    {
                                        listQuality.DataBarcode = barcode.Value;

                                    }
                                    listQuality.DateTime = f.Bucket.AddHours(7);

                                    dt.Add(listQuality);

                                }
                            }
                        }
                        break;
                    case "year":

                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var volumeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                            AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                            AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = volumeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                            AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                            AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = barcodeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (volumeConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityCoolantFilingDto
                                {
                                    DateTime = DateTime.Now,
                                    VolumeCoolant = 0,
                                    DataBarcode = "-",

                                };
                            }
                            else
                            {

                                foreach (var f in barcodeConsumption)
                                {
                                    GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                    var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (vol != null)
                                    {
                                        listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                    }
                                    var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                    if (barcode != null)
                                    {
                                        listQuality.DataBarcode = barcode.Value;

                                    }
                                    listQuality.DateTime = f.Bucket.AddHours(7);

                                    dt.Add(listQuality);

                                }
                            }
                        }
                        break;
                    default:

                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var volumeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = volumeVid.Subject.Vid, now = DateTime.Now.Date });

                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                            (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = barcodeVid.Subject.Vid, now = DateTime.Now.Date });

                            if (volumeConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityCoolantFilingDto
                                {
                                    DateTime = DateTime.Now,
                                    VolumeCoolant = 0,
                                    DataBarcode = "-",

                                };
                            }
                            else
                            {

                                foreach (var f in barcodeConsumption)
                                {
                                    GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                    var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (vol != null)
                                    {
                                        listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                    }
                                    var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                    if (barcode != null)
                                    {
                                        listQuality.DataBarcode = barcode.Value;

                                    }
                                    listQuality.DateTime = f.Bucket.AddHours(7);

                                    dt.Add(listQuality);

                                }
                            }
                        }
                        break;
                }

                var paginatedList = dt.Where(c => query.search_term == null || query.search_term == c.DataBarcode.ToString()
                || query.search_term == c.VolumeCoolant.ToString())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
