using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination
{

    public record GetListQualityNutRunnerSteeringStemQuery : IRequest<PaginatedResult<GetListQualityNutRunnerSteeringStemDto>>
    {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }

        public GetListQualityNutRunnerSteeringStemQuery() { }

        public GetListQualityNutRunnerSteeringStemQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string types, DateTime startTime, DateTime endTime)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = types;
            start = startTime;
            end = endTime;
        }

        internal class GetListQualityNutRunnerStreeringStemQueryHandler : IRequestHandler<GetListQualityNutRunnerSteeringStemQuery, PaginatedResult<GetListQualityNutRunnerSteeringStemDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityNutRunnerStreeringStemQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityNutRunnerSteeringStemDto>> Handle(GetListQualityNutRunnerSteeringStemQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();

                List<GetListQualityNutRunnerSteeringStemDto> dt = new List<GetListQualityNutRunnerSteeringStemDto>();
                var data = new GetListQualityNutRunnerSteeringStemDto();

                var bcVid = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
                var statusVid = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
                var torsiVid = machine.Where(m => m.Subject.Vid.Contains("TORQ")).FirstOrDefault();

                switch (query.type)
                {
                    case "day":
                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });  
                            
                            var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {

                                foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if(TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if(Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }
                        
                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
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
                    }
                            break;
                    case "week":
                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                            AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                            AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                            AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (statusConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityNutRunnerSteeringStemDto
                                {
                                    DateTime = DateTime.Now,
                                    Status = "-",
                                    DataBarcode = "-",
                                    DataTorQ = 0
                                };
                            }
                            else
                            {

                                foreach (var f in statusConsumption)
                                {
                                    GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                    var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (TorQ != null)
                                    {
                                        listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                    }
                                    var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                    if (Barcode != null)
                                    {
                                        listQuality.DataBarcode = Barcode.Value;
                                    }

                                    var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                    if (statuss != null && statuss.Value.Contains("1"))
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
                        }
                        break;
                    case "month":
                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                            AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                            AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                            AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (statusConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityNutRunnerSteeringStemDto
                                {
                                    DateTime = DateTime.Now,
                                    Status = "-",
                                    DataBarcode = "-",
                                    DataTorQ = 0
                                };
                            }
                            else
                            {

                                foreach (var f in statusConsumption)
                                {
                                    GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                    var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (TorQ != null)
                                    {
                                        listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                    }
                                    var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                    if (Barcode != null)
                                    {
                                        listQuality.DataBarcode = Barcode.Value;
                                    }

                                    var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                    if (statuss != null && statuss.Value.Contains("1"))
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
                        }
                        break;
                    case "year":
                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                            AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                            AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                            AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (statusConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityNutRunnerSteeringStemDto
                                {
                                    DateTime = DateTime.Now,
                                    Status = "-",
                                    DataBarcode = "-",
                                    DataTorQ = 0
                                };
                            }
                            else
                            {

                                foreach (var f in statusConsumption)
                                {
                                    GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                    var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (TorQ != null)
                                    {
                                        listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                    }
                                    var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                    if (Barcode != null)
                                    {
                                        listQuality.DataBarcode = Barcode.Value;
                                    }

                                    var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                    if (statuss != null && statuss.Value.Contains("1"))
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
                        }
                        break;
                    default:
                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = bcVid.Subject.Vid, now = DateTime.Now.Date });

                            var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = torsiVid.Subject.Vid, now = DateTime.Now.Date });

                            var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = statusVid.Subject.Vid, now = DateTime.Now.Date, });

                            if (statusConsumption.Count() == 0)
                            {
                                data =
                                new GetListQualityNutRunnerSteeringStemDto
                                {
                                    DateTime = DateTime.Now,
                                    Status = "-",
                                    DataBarcode = "-",
                                    DataTorQ = 0
                                };
                            }
                            else
                            {

                                foreach (var f in statusConsumption)
                                {
                                    GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                    var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (TorQ != null)
                                    {
                                        listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                    }
                                    var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                    if (Barcode != null)
                                    {
                                        listQuality.DataBarcode = Barcode.Value;
                                    }

                                    var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                    if (statuss != null && statuss.Value.Contains("1"))
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
                        }
                        break;

                }
             
                var paginatedList = dt.Where(c => query.search_term == null || query.search_term.ToLower() == c.DataBarcode.ToLower() 
                || query.search_term.ToLower() == c.Status.ToLower())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }

}

