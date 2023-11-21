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

        public GetListQualityNutRunnerSteeringStemQuery() { }

        public GetListQualityNutRunnerSteeringStemQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
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

                var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
                var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
                var torsi = machine.Where(m => m.Subject.Vid.Contains("TORQ")).FirstOrDefault();

                var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                         (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                         new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                var torsiConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                       (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                       new { vid = torsi.Subject.Vid, now = DateTime.Now.Date });

                var statusConsumption = await _dapperReadDbConnection.QueryAsync<ListQualityConsumption>
                       (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                       AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                       ORDER BY  bucket DESC",
                       new { vid = status.Subject.Vid, nom = DateTime.Now.Date, });

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

             
                var paginatedList = dt.Where(c => query.search_term == null || query.search_term.ToLower() == c.DataBarcode.ToLower() 
                || query.search_term.ToLower() == c.Status.ToLower())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }

}

