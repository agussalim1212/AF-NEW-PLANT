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

        public GetListQualityCoolantFilingQuery() { }

        public GetListQualityCoolantFilingQuery(string searchTerm, Guid machineId, int pageNumber, int pageSize)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
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

                //IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();

                List<GetListQualityCoolantFilingDto> dt = new List<GetListQualityCoolantFilingDto>();
                var data = new GetListQualityCoolantFilingDto();

                var v = machine.Where(m => m.Subject.Vid.Contains("VOL-COLN")).FirstOrDefault();
                var x = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();

                var volumeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                         (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC",
                         new { vid = v.Subject.Vid, now = DateTime.Now.Date });

                var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                       (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                       new { vid = x.Subject.Vid, now = DateTime.Now.Date });



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

                var paginatedList = dt.Where(c => query.search_term == null || query.search_term == c.DataBarcode.ToString()
                || query.search_term == c.VolumeCoolant.ToString())
               .ToList();

                return await paginatedList.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}
