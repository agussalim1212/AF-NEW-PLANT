using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination
{
    public record GetListWheelRearQuery : IRequest<PaginatedResult<GetListWheelRearDto>>
    {
        public Guid machine_id { get; set; }
        public string type_wheel { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }

        public GetListWheelRearQuery() { }

        public GetListWheelRearQuery(string typesWheel, string searchTerm, Guid machineId, int pageNumber, int pageSize, string Type, DateTime Start, DateTime End)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type_wheel = typesWheel;
            type = Type;
            start = Start; 
            end = End;
        }

        internal class GetListWheelRearQueryHandler : IRequestHandler<GetListWheelRearQuery, PaginatedResult<GetListWheelRearDto>>
        {
            private readonly IDayRepository _dayRepository;
            private readonly IDefaultRepository _defaultRepository;
            private readonly IDetailMachineRepository _detailMachineRepository;
            public GetListWheelRearQueryHandler(IDayRepository dayRepository, IDefaultRepository defaultRepository, IDetailMachineRepository detailMachineRepository)
            {
                _dayRepository = dayRepository;
                _defaultRepository = defaultRepository;
                _detailMachineRepository = detailMachineRepository;
            }

            public async Task<PaginatedResult<GetListWheelRearDto>> Handle(GetListWheelRearQuery query, CancellationToken cancellationToken)
            {
                List<GetListWheelRearDto> data = new List<GetListWheelRearDto>();

                //Final Inspection
                var vidStatus = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-STATUS-PRODUKSI");
                var vidHorizontal = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-HORIZONTAL");
                var vidVertikal = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-VERTIKAL");
                var vidDiskBrake = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-DISK-BRAKE");

                //Tire Inflation
                var vidTire = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "TIQ-TIRE-PRASSURE");


                if (query.type == "day" || query.type == "week" || query.type == "month" || query.type == "year")
                {
                    if (query.type_wheel == "final_inspection")
                    {
                        var day = await _dayRepository.GetListQualityAssyWheelRearFinalInspectioDay(vidStatus.Vid, vidHorizontal.Vid, vidVertikal.Vid, vidDiskBrake.Vid, query.machine_id, query.start, query.end);
                        var paged = day.Where(c => (query.search_term == null)
                          || (query.search_term.ToLower() == c.Status.ToLower())
                          || (query.search_term.ToLower() == c.DataDialHorizontal.ToLower())
                          || (query.search_term.ToLower() == c.DataDialVertical.ToLower())
                          || (query.search_term.ToLower() == c.DiskBrake.ToLower())).ToList();
                        data.AddRange(paged);
                    }
                    else
                    {
                        var day = await _dayRepository.GetListQualityAssyWheelRearTireInflationDay(vidTire.Vid, query.type_wheel, query.search_term, query.machine_id, query.start, query.end);
                        var paged = day.Where(c => (query.search_term == null)
                          || (query.search_term.ToLower() == c.TirePresure.ToLower())).ToList();
                        data.AddRange(paged);
                    }
                }
                else
                {
                    if (query.type_wheel == "final_inspection")
                    {
                        var day = await _defaultRepository.GetListQualityAssyWheelRearFinalInspectionDefault(vidStatus.Vid, vidHorizontal.Vid, vidVertikal.Vid, vidDiskBrake.Vid, query.machine_id, query.start, query.end);
                        var paged = day.Where(c => (query.search_term == null)
                          || (query.search_term.ToLower() == c.Status.ToLower())
                          || (query.search_term.ToLower() == c.DataDialHorizontal.ToLower())
                          || (query.search_term.ToLower() == c.DataDialVertical.ToLower())
                          || (query.search_term.ToLower() == c.DiskBrake.ToLower())).ToList();
                        data.AddRange(paged);
                    }
                    else
                    {
                        var defaultData = await _defaultRepository.GetListQualityAssyWheelRearTireInflationDefault(vidTire.Vid, query.type_wheel, query.search_term, query.machine_id, query.start, query.end);
                        var paged = defaultData.Where(c => (query.search_term == null)
                           || (query.search_term.ToLower() == c.TirePresure.ToLower())).ToList();
                        data.AddRange(paged);
                    }
                }

                return await data.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}