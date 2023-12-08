using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
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
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public GetListQualityMainLineQuery() { }

        public GetListQualityMainLineQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize, string types, DateTime startTime, DateTime endTime)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type = types;
            start = startTime;
            end = endTime;
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

                List<GetListQualityMainLineDto> dt = new List<GetListQualityMainLineDto>();
                var data = new GetListQualityMainLineDto();

                var timeVid = machine.Where(m => m.Subject.Vid.Contains("TIME-OPARATION")).FirstOrDefault();
                var frqVid = machine.Where(m => m.Subject.Vid.Contains("FRQ_INVERT")).FirstOrDefault();

                switch (query.type)
                {
                    case "day":
                        if (query.end.Date < query.start.Date)
                        {
                            throw new ArgumentException("End day cannot be earlier than start date.");
                        }
                        else
                        {
                            var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = timeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = frqVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (timeConsumption.Count() == 0)
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

                                foreach (var f in timeConsumption)
                                {
                                    GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                    var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (frQ != null)
                                    {
                                        listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                    }
                                    var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                    if (inverter != null)
                                    {
                                        listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
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
                            var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = timeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = frqVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (timeConsumption.Count() == 0)
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

                                foreach (var f in timeConsumption)
                                {
                                    GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                    var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (frQ != null)
                                    {
                                        listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                    }
                                    var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                    if (inverter != null)
                                    {
                                        listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
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
                            var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = timeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = frqVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (timeConsumption.Count() == 0)
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

                                foreach (var f in timeConsumption)
                                {
                                    GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                    var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (frQ != null)
                                    {
                                        listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                    }
                                    var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                    if (inverter != null)
                                    {
                                        listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
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
                            var timeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = timeVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            var frqConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                            ORDER BY id DESC, bucket DESC", new { vid = frqVid.Subject.Vid, starttime = query.start.Date, endtime = query.end.Date });

                            if (timeConsumption.Count() == 0)
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

                                foreach (var f in timeConsumption)
                                {
                                    GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                    var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (frQ != null)
                                    {
                                        listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                    }
                                    var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                    if (inverter != null)
                                    {
                                        listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
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
                            var timeConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = timeVid.Subject.Vid, now = DateTime.Now.Date });

                            var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                            (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                            new { vid = frqVid.Subject.Vid, now = DateTime.Now.Date });

                            if (timeConsumption.Count() == 0)
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

                                foreach (var f in timeConsumption)
                                {
                                    GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                    var frQ = frqConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                    if (frQ != null)
                                    {
                                        listQuality.FrqInverter = Convert.ToDecimal(frQ.Value);
                                    }
                                    var inverter = timeConsumption.Where(k => k.Bucket == frQ.Bucket).FirstOrDefault();
                                    if (inverter != null)
                                    {
                                        listQuality.DurationStop = Convert.ToDecimal(inverter.Value);
                                    }
                                    listQuality.DateTime = f.Bucket.AddHours(7);
                                    dt.Add(listQuality);

                                }
                            }
                        }

                        break;
                }
                var paginatedList = dt.Where(c => query.search_term == null || query.search_term == c.FrqInverter.ToString()
                || query.search_term == c.DurationStop.ToString())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
