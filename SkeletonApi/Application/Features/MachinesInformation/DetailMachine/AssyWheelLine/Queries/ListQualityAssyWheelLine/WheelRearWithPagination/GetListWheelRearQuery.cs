using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination
{
    public record GetListWheelRearQuery : IRequest<PaginatedResult<GetListWheelRearDto>>
    {
        public Guid machine_id { get; set; }
        public string type_wheel { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string? search_term { get; set; }

        public GetListWheelRearQuery() { }

        public GetListWheelRearQuery(string typesWheel, string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type_wheel = typesWheel;
        }

        internal class GetListWheelRearQueryHandler : IRequestHandler<GetListWheelRearQuery, PaginatedResult<GetListWheelRearDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListWheelRearQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListWheelRearDto>> Handle(GetListWheelRearQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();


                List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
                var data = new GetListWheelRearDto();

                var tire = machine.Where(m => m.Subject.Vid.Contains("TIRE-PRESURE")).FirstOrDefault();
                var brngDistance = machine.Where(m => m.Subject.Vid.Contains("DISTANCE")).FirstOrDefault();
                var brngTonase = machine.Where(m => m.Subject.Vid.Contains("TONASE")).FirstOrDefault();
                var horizontal = machine.Where(m => m.Subject.Vid.Contains("DIAL-HOR")).FirstOrDefault();
                var vertikal = machine.Where(m => m.Subject.Vid.Contains("DIAL-VER")).FirstOrDefault();
                var torQ = machine.Where(m => m.Subject.Vid.Contains("TORQ")).FirstOrDefault();
                var statusPressConeRace = machine.Where(m => m.Subject.Vid.Contains("BRNG-STATUS-PRDCT")).FirstOrDefault();
                var statusInspection = machine.Where(m => m.Subject.Vid.Contains("INPECT-STATUS-PRDCT")).FirstOrDefault();


                var tireConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                ORDER BY  bucket DESC",
                new { vid = tire.Subject.Vid, now = DateTime.Now.Date });

                var brngDistnaceConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                ORDER BY  bucket DESC",
                new { vid = brngDistance.Subject.Vid, now = DateTime.Now.Date });

                var brngTonaseConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                ORDER BY  bucket DESC",
                new { vid = brngTonase.Subject.Vid, now = DateTime.Now.Date });

                var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                ORDER BY  bucket DESC",
                new { vid = horizontal.Subject.Vid, now = DateTime.Now.Date });

                var vertikalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                ORDER BY  bucket DESC",
                new { vid = vertikal.Subject.Vid, now = DateTime.Now.Date });

                var statusPressConeRaceConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                ORDER BY  bucket DESC",
                new { vid = statusPressConeRace.Subject.Vid, dateNow = DateTime.Now.Date, });

                var statusInspectionConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                   (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                ORDER BY  bucket DESC",
                new { vid = statusInspection.Subject.Vid, dateNow = DateTime.Now.Date, });

                var torQConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                ORDER BY  bucket DESC",
                new { vid = torQ.Subject.Vid, dateNow = DateTime.Now.Date, });


                switch (query.type_wheel)
                {
                    case "final_inspection":

                        if (statusInspectionConsumption.Count() == 0)
                        {
                            data =
                            new GetListWheelRearDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataDistance = 0,
                                DataTonase = 0,
                            };
                            dt.Add(data);
                        }
                        else
                        {

                            foreach (var s in statusInspectionConsumption)
                            {
                                GetListWheelRearDto listQuality = new GetListWheelRearDto();

                                var dataDialHorizontal = horizontalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (dataDialHorizontal != null)
                                {
                                    listQuality.DataDialHorizontal = Convert.ToDecimal(dataDialHorizontal.Value);
                                }
                                var dataDialVertikal = vertikalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (dataDialVertikal != null)
                                {
                                    listQuality.DataDialVertical = Convert.ToDecimal(dataDialVertikal.Value);
                                }
                                var statuss = statusInspectionConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
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
                        break;
                    case "tire_inflation":

                        if (tireConsumption.Count() == 0)
                        {
                            data =
                            new GetListWheelRearDto
                            {
                                DateTime = DateTime.Now,
                                TirePresure = 0,
                            };
                            dt.Add(data);
                        }
                        else
                        {

                            foreach (var s in tireConsumption)
                            {
                                GetListWheelRearDto listQuality = new GetListWheelRearDto();

                                    listQuality.TirePresure = Convert.ToDecimal(s.Value);

                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                                //dt.Where(d => query.search_term == null || query.search_term == d.TirePresure.ToString());
                            }
                        }
                        break;
                    case "disk_brake":

                        if (torQConsumption.Count() == 0)
                        {
                            data =
                            new GetListWheelRearDto
                            {
                                DateTime = DateTime.Now,
                                DataTorQ = 0
                            };
                            dt.Add(data);
                        }
                        else
                        {
                            foreach (var s in torQConsumption)
                            {
                                GetListWheelRearDto listQuality = new GetListWheelRearDto();

                                listQuality.DataTorQ = Convert.ToDecimal(s.Value);
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                        break;
                    default:

                        if (statusPressConeRaceConsumption.Count() == 0)
                        {
                            data =
                            new GetListWheelRearDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataDistance = 0,
                                DataTonase = 0,
                            };
                            dt.Add(data);
                        }
                        else
                        {

                            foreach (var s in statusPressConeRaceConsumption)
                            {
                                GetListWheelRearDto listQuality = new GetListWheelRearDto();

                                var dataDial = brngDistnaceConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (dataDial != null)
                                {
                                    listQuality.DataDistance = Convert.ToDecimal(dataDial.Value);
                                }
                                var dataTonase = brngTonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (dataTonase != null)
                                {
                                    listQuality.DataTonase = Convert.ToDecimal(dataTonase.Value);
                                }
                                var statuss = statusPressConeRaceConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
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
                        break;
                }

                var paginatedList = dt.Where(c => query.search_term == null
                || (query.search_term == c.DataDistance.ToString())
                || (query.search_term.ToLower() == c.Status.ToLower())).ToList();
                //|| (query.search_term == c.DataTonase.ToString())
                //|| (query.search_term == c.DataDialHorizontal.ToString())
                //|| (query.search_term == c.DataDialVertical.ToString())
                //|| (query.search_term == c.TirePresure.ToString())
                //|| (query.search_term == c.DataTorQ.ToString())).ToList();

                return await dt.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}