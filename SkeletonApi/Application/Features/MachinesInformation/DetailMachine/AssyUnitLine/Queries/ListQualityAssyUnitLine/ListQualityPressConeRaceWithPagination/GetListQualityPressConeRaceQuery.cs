using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRace
{
        public record GetListQualityPressConeRaceQuery : IRequest<PaginatedResult<GetListQualityPressConeRaceDto>>
        {
        public Guid machine_id { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }

        public string? search_term { get; set; }

        public GetListQualityPressConeRaceQuery() { }

        public GetListQualityPressConeRaceQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
        }

        internal class GetListQualityPressConeRaceQueryHandler : IRequestHandler<GetListQualityPressConeRaceQuery, PaginatedResult<GetListQualityPressConeRaceDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IDapperReadDbConnection _dapperReadDbConnection;

            public GetListQualityPressConeRaceQueryHandler(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _dapperReadDbConnection = dapperReadDbConnection;
            }

            public async Task<PaginatedResult<GetListQualityPressConeRaceDto>> Handle(GetListQualityPressConeRaceQuery query, CancellationToken cancellationToken)
            {
                var machine = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Include(s => s.Machine).Include(s => s.Subject)
                .Where(m => (query.machine_id == m.MachineId)).ToListAsync();


                List<GetListQualityPressConeRaceDto> dt = new List<GetListQualityPressConeRaceDto>();
                var data = new GetListQualityPressConeRaceDto();

                var kedalaman = machine.Where(m => m.Subject.Vid.Contains("DEPTH")).FirstOrDefault();
                var tonase = machine.Where(m => m.Subject.Vid.Contains("TONASE")).FirstOrDefault();

                var kedalamanConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                         (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                         new { vid = kedalaman.Subject.Vid, now = DateTime.Now.Date });

                var tonaseConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                      (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                      new { vid = tonase.Subject.Vid, now = DateTime.Now.Date });


                if (kedalamanConsumption.Count() == 0)
                {
                    data =
                    new GetListQualityPressConeRaceDto
                    {
                        DateTime = DateTime.Now,
                        Kedalaman = 0,
                        Tonase = 0

                    };
                }
                else
                {

                    foreach (var s in kedalamanConsumption)
                    {
                        GetListQualityPressConeRaceDto listQuality = new GetListQualityPressConeRaceDto();

                        var Depth = tonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                        if (Depth != null)
                        {
                            listQuality.Kedalaman = Convert.ToDecimal(s.Value);
                        }
                        var Tonasee = kedalamanConsumption.Where(k => k.Bucket == Depth.Bucket).FirstOrDefault();
                        if (Tonasee != null)
                        {
                            listQuality.Tonase = Convert.ToDecimal(Tonasee.Value);
                        }
                        listQuality.DateTime = s.Bucket.AddHours(7);
                        dt.Add(listQuality);

                    }
                }

                var paginatedList = dt.Where(c => query.search_term == null || query.search_term == c.Tonase.ToString()
                || query.search_term == c.Kedalaman.ToString())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
